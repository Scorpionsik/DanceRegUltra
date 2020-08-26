using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
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
            this.Title = "Менеджер событий - " + App.AppTitle;
            this.TitleUpdate_Callback = new TimerCallback(this.UpdateEventTitleMethod);
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
                await DanceRegDatabase.ExecuteNonQueryAsync("update events set " + column_name + "=" + value_update + " where Id_event=" + event_id);
            }
        }
    }
}
