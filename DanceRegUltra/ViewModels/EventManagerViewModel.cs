using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Views.EventManagerViews;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class EventManagerViewModel : ViewModel
    {
        private TimerCallback TitleUpdate_Callback;
        private Timer TitleUpdate_Timer;



        public DanceEvent EventInWork { get; private set; }

        public override string Title
        {
            get => "[" + this.EventEditTitle + "] " + base.Title;
        }

        private string eventEditTitle;
        public string EventEditTitle
        {
            get => this.eventEditTitle;
            set
            {
                this.eventEditTitle = value;
                if (this.TitleUpdate_Timer != null) this.TitleUpdate_Timer.Dispose();
                this.TitleUpdate_Timer = new Timer(this.TitleUpdate_Callback, null, 1000, 0);
                this.OnPropertyChanged("EventEditTitle");
                this.OnPropertyChanged("Title");
            }
        }

        private DateTime startDateEvent;
        public DateTime StartDateEvent
        {
            get => this.startDateEvent;
            set
            {
                this.startDateEvent = value;
                this.OnPropertyChanged("StartDateEvent");
                this.EventInWork.SetStartTimestampEvent(UnixTime.ToUnixTimestamp(new DateTimeOffset(value, App.Locality)));
            }
        }

        public EventManagerViewModel(int idEventLoad)
        {
            this.EventInWork = DanceRegCollections.GetEventById(idEventLoad);
            this.EventInWork.Event_UpdateDanceEvent += UpdateEvent;
            this.eventEditTitle = this.EventInWork.Title;
            this.startDateEvent = UnixTime.ToDateTimeOffset(this.EventInWork.StartEventTimestamp, App.Locality).DateTime;
            this.Title = "Менеджер событий - " + App.AppTitle;
            this.TitleUpdate_Callback = new TimerCallback(this.UpdateEventTitleMethod);
            this.Initialize();
        }

        private async void Initialize()
        {
            DbResult res = null;
            foreach(JsonSchemeArray platform in this.EventInWork.SchemeEvent.Platforms)
            {
                foreach(IdCheck league in platform.Values)
                {
                    if (league.IsChecked)
                    {
                        if (!this.EventInWork.Leagues.ContainsKey(league.Id))
                        {
                            this.EventInWork.Leagues.Add(league.Id, new List<IdTitle>());
                            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from leagues where Id_league=" + league.Id);
                            DanceRegCollections.LoadLeague(new CategoryString(res["Id_league", 0].ToInt32(), CategoryType.League, res["Name", 0].ToString(), res["Position", 0].ToInt32(), res["IsHide", 0].ToBoolean()));
                        }
                        this.EventInWork.Leagues[league.Id].Add(new IdTitle(platform.IdArray, platform.Title));
                    }
                }
            }

            foreach(JsonSchemeArray block in this.EventInWork.SchemeEvent.Blocks)
            {
                foreach(IdCheck age in block.Values)
                {
                    if (age.IsChecked)
                    {
                        if (!this.EventInWork.Ages.ContainsKey(age.Id))
                        {
                            this.EventInWork.Ages.Add(age.Id, new List<IdTitle>());
                            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from ages where Id_age=" + age.Id);
                            DanceRegCollections.LoadAge(new CategoryString(res["Id_age", 0].ToInt32(), CategoryType.Age, res["Name", 0].ToString(), res["Position", 0].ToInt32(), res["IsHide", 0].ToBoolean()));
                        }
                        this.EventInWork.Ages[age.Id].Add(new IdTitle(block.IdArray, block.Title));
                    }
                }
            }

            foreach(IdCheck style in this.EventInWork.SchemeEvent.Styles)
            {
                if (style.IsChecked)
                {
                    this.EventInWork.Styles.Add(style.Id);
                    res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from styles where Id_style=" + style.Id);
                    DanceRegCollections.LoadStyle(new CategoryString(res["Id_style", 0].ToInt32(), CategoryType.Style, res["Name", 0].ToString(), res["Position", 0].ToInt32(), res["IsHide", 0].ToBoolean()));
                }
            }
        }

        private void UpdateEventTitleMethod(object obj)
        {
            if (this.TitleUpdate_Timer != null) this.TitleUpdate_Timer.Dispose();
            this.EventInWork.SetTitle(this.EventEditTitle);
        }

        public override WindowClose CloseMethod()
        {
            this.EventInWork.Event_UpdateDanceEvent -= UpdateEvent;
            DanceRegCollections.UnloadEvent(this.EventInWork);
            return base.CloseMethod();
        }

        private async static void UpdateEvent(int event_id, string column_name, object value_update)
        {
            if (event_id > 0)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("update events set " + column_name + "='" + value_update + "' where Id_event=" + event_id);
            }
        }

        public RelayCommand Command_AddDancer
        {
            get => new RelayCommand(obj =>
            {
                AddDancerView window = new AddDancerView(this.EventInWork.IdEvent);
                window.ShowDialog();
            });
        }
    }
}
