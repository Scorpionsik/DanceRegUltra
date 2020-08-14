using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Static;
using DanceRegUltra.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public ListExt<DanceEvent> Events { get => DanceRegCollections.Events; }

        private DanceEvent select_event;
        public DanceEvent Select_event
        {
            get => this.select_event;
            set
            {
                this.select_event = value;
                this.OnPropertyChanged("Select_event");
            }
        }

        public StatusString Status { get; private set; }

        private int countDatabaseRequests;
        public int CountDatabaseRequests
        {
            get => this.countDatabaseRequests;
            private set
            {
                this.countDatabaseRequests = value;
                this.OnPropertyChanged("CountDatabaseRequests");
            }
        }

        public MainViewModel() : base()
        {
            this.Title = "Создание или выбор события - " + App.AppTitle;

            this.Status = new StatusString();
            this.Initialize();
        }

        private async void Initialize()
        {
            DanceRegDatabase.Event_StartTask += this.StartDbTask;
            DanceRegDatabase.Event_EndTask += this.EndDbTask;

            if (!DanceRegDatabase.IsExist())
            {
                await this.Status.SetAsync("Добро пожаловать! Инициализация первого запуска...", StatusString.Infinite);
                //await Task.Run(() => Thread.Sleep(1000));
                MatchCollection commands = DanceRegDatabase.DatabaseCommands;

                foreach(Match command in commands)
                {
                    await DanceRegDatabase.ExecuteNonQueryAsync(command.Value);
                }
            }
            
            DbResult db_events = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_event, Title, Start_timestamp, End_timestamp from events order by Start_timestamp");
            foreach(DbRow row in db_events)
            {
                DanceRegCollections.Events.Add(new DanceEvent(row["Id_event"].ToInt32(), row["Title"].ToString(), row["Start_timestamp"].ToDouble(), row["End_timestamp"].ToDouble()));
            }
            DanceRegCollections.Events.CollectionChanged += this.UpdateEvents;

            await this.Status.SetAsync("Готов к работе!", StatusString.LongTime);
        }

        private void UpdateEvents(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Events");
        }

        private void StartDbTask() => this.CountDatabaseRequests++;
        private void EndDbTask() => this.CountDatabaseRequests--;

        private static async void DeleteEvent(DanceEvent deleteEvent)
        {
            DanceRegCollections.Events.Remove(deleteEvent);
            await DanceRegDatabase.ExecuteNonQueryAsync("delete from events where Id_event=" + deleteEvent.IdEvent);
        }

        private async void InitializeEvent(DanceEvent init_event)
        {
            await this.Status.SetAsync("Инициализация события " + init_event.Title + "...", StatusString.Infinite);




            await DanceRegDatabase.ExecuteNonQueryAsync("insert into events ('Title', 'Start_timestamp', 'Json_scheme') values ('" + init_event.Title + "', " + init_event.StartEventTimestamp + ", '"+ init_event.JsonSchemeEvent +"')");
            DbResult new_event = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from events order by Id_event");
            DbRow current_row = new_event[new_event.RowsCount - 1];
            DanceEvent newEvent = new DanceEvent(current_row["Id_event"].ToInt32(), current_row["Title"].ToString(), current_row["Start_timestamp"].ToDouble(), current_row["End_timestamp"].ToDouble(), current_row["Json_scheme"].ToString());
            int sort_id = 0;
            while (sort_id < DanceRegCollections.Events.Count && DanceRegCollections.Events[sort_id].CompareTo(newEvent) <= 0) sort_id++;
            DanceRegCollections.Events.Insert(sort_id, newEvent);

            await this.Status.SetAsync("Готово! Открываю " + init_event.Title + "...", StatusString.LongTime);
        }

        public RelayCommand Command_AddEvent
        {
            get => new RelayCommand(obj =>
            {
                //AddNewEvent();
                RegisterWindowView window = new RegisterWindowView();
                if ((bool)window.ShowDialog())
                {
                    this.InitializeEvent(window.Return_event);
                }
            });
        }

        public static RelayCommand<DanceEvent> Command_EditEvent
        {
            get => new RelayCommand<DanceEvent>(ev =>
            {

            },
               (ev) => ev != null);
        }

        public static RelayCommand<DanceEvent> Command_DeleteEvent
        {
            get => new RelayCommand<DanceEvent>(ev =>
            {
                DeleteEvent(ev);
            },
                (ev) => ev != null);
        }

        public static RelayCommand Command_LoadSchemeManager
        {
            get => new RelayCommand(obj =>
            {
                SchemeManagerView window = new SchemeManagerView();
                window.Show();
            });
        }

        public static RelayCommand<string> Command_OpencategoryWindow
        {
            get => new RelayCommand<string>(param =>
            {
                CategoryType type = CategoryType.League;
                switch (param)
                {
                    case "league":
                        type = CategoryType.League;
                        break;
                    case "age":
                        type = CategoryType.Age;
                        break;
                    case "style":
                        type = CategoryType.Style;
                        break;
                }

                CategoryManagerView window = new CategoryManagerView(type);
                window.Show();
            });
        }
    }
}
 