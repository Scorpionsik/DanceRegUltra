using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
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
            this.Title = App.AppTitle + ": Создание или выбор события";

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
            
            DbResult db_events = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_event, Title, Start_timestamp, End_timestamp from events order by Id_event");
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

        /// <summary>
        /// В будущем удалить, вместо этой функции надо вызывать окно для редактирования событий
        /// </summary>
        private static async void AddNewEvent()
        {
            await DanceRegDatabase.ExecuteNonQueryAsync("insert into events ('Title', 'Start_timestamp') values ('test', " + UnixTime.CurrentUnixTimestamp() + ")");
            DbResult new_event = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_event, Title, Start_timestamp, End_timestamp from events order by Id_event");
            DbRow current_row = new_event[new_event.RowsCount - 1];
            DanceRegCollections.Events.Add(new DanceEvent(current_row["Id_event"].ToInt32(), current_row["Title"].ToString(), current_row["Start_timestamp"].ToDouble(), current_row["End_timestamp"].ToDouble()));
        }

        private static async void DeleteEvent(DanceEvent deleteEvent)
        {
            DanceRegCollections.Events.Remove(deleteEvent);
            await DanceRegDatabase.ExecuteNonQueryAsync("delete from events where Id_event=" + deleteEvent.IdEvent);
        }

        public static RelayCommand Command_AddEvent
        {
            get => new RelayCommand(obj =>
            {
                AddNewEvent();
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
    }
}
 