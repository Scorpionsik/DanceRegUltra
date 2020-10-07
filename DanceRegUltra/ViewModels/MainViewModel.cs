using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Views;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
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

        public static StatusString Status { get; private set; }

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

            Status = new StatusString();
            this.Initialize();
        }

        private async void Initialize()
        {
            DanceRegDatabase.Event_StartTask += this.StartDbTask;
            DanceRegDatabase.Event_EndTask += this.EndDbTask;

            if (!DanceRegDatabase.IsExist())
            {
                await Status.SetAsync("Добро пожаловать! Инициализация первого запуска...", StatusString.Infinite);
                //await Task.Run(() => Thread.Sleep(1000));
                MatchCollection commands = DanceRegDatabase.DatabaseCommands;

                foreach(Match command in commands)
                {
                    await DanceRegDatabase.ExecuteNonQueryAsync(command.Value);
                }
            }
            
            DbResult db_events = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from events order by Start_timestamp");
            foreach(DbRow row in db_events)
            {
                DanceEvent tmp_add = new DanceEvent(row.GetInt32("Id_event"), row["Title"].ToString(), row.GetDouble("Start_timestamp"), row.GetDouble("End_timestamp"), row["Json_scheme"].ToString(), row.GetInt32("Id_node_increment"));
                tmp_add.SetEnableRandNums(row.GetBoolean("Id_event"));
                tmp_add.All_members_count = row.GetInt32("All_member_count");
                tmp_add.Event_UpdateTimeDate += UpdateEventPosition;
                DanceRegCollections.Events.Add(tmp_add);
            }
            DanceRegCollections.Events.CollectionChanged += this.UpdateEvents;

            await Status.SetAsync("Готов к работе!", StatusString.LongTime);
        }

        private void UpdateEvents(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("Events");
        }

        private void StartDbTask() => this.CountDatabaseRequests++;
        private void EndDbTask() => this.CountDatabaseRequests--;

        private static async void DeleteEvent(DanceEvent deleteEvent)
        {
            deleteEvent.Event_UpdateTimeDate -= UpdateEventPosition;
            DanceRegCollections.Events.Remove(deleteEvent);
            await DanceRegDatabase.ExecuteNonQueryAsync("delete from events where Id_event=" + deleteEvent.IdEvent);
            await DanceRegDatabase.ExecuteNonQueryAsync("delete from event_nodes where Id_event=" + deleteEvent.IdEvent);
            await DanceRegDatabase.ExecuteNonQueryAsync("delete from nominations where Id_event=" + deleteEvent.IdEvent);
            await DanceRegDatabase.ExecuteNonQueryAsync("delete from nums_for_members where Id_event=" + deleteEvent.IdEvent);
        }

        private async void InitializeEvent(DanceEvent init_event)
        {
            await Status.SetAsync("Инициализация события " + init_event.Title + "...", StatusString.Infinite);
                                 
            await DanceRegDatabase.ExecuteNonQueryAsync("insert into events ('Title', 'Start_timestamp', 'Json_scheme') values ('" + init_event.Title + "', " + init_event.StartEventTimestamp + ", '"+ JsonScheme.Serialize(init_event.SchemeEvent) +"')");
            DbResult new_event = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from events order by Id_event");
            DbRow current_row = new_event.GetRow(new_event.RowsCount - 1);
            DanceEvent newEvent = new DanceEvent(current_row.GetInt32("Id_event"), current_row["Title"].ToString(), current_row.GetDouble("Start_timestamp"), current_row.GetDouble("End_timestamp"), current_row["Json_scheme"].ToString());
            newEvent.SetEnableRandNums(current_row.GetBoolean("Enable_rand_nums"));
            newEvent.Event_UpdateTimeDate += UpdateEventPosition;
            int sort_id = 0;
            while (sort_id < DanceRegCollections.Events.Count && DanceRegCollections.Events[sort_id].CompareTo(newEvent) <= 0) sort_id++;
            DanceRegCollections.Events.Insert(sort_id, newEvent);

            await Status.SetAsync("Готово! Открываю " + newEvent.Title + "...", StatusString.LongTime);
            DanceRegCollections.LoadEvent(newEvent);
            DanceRegCollections.Active_events_windows.Value[newEvent.IdEvent].Show();
        }

        public override WindowClose CloseMethod()
        {
            List<EventManagerView> closeEvents = new List<EventManagerView>(DanceRegCollections.Active_events_windows.Value.Values);
            foreach(EventManagerView closeEvent in closeEvents)
            {
                closeEvent.Close();
            }
            return base.CloseMethod();
        }

        private static void UpdateEventPosition(DanceEvent update_event)
        {
            int old_index = DanceRegCollections.Events.IndexOf(update_event);
            int new_index = 0;
            while (new_index < DanceRegCollections.Events.Count && DanceRegCollections.Events[new_index].CompareTo(update_event) <= 0) new_index++;
            if (new_index == DanceRegCollections.Events.Count) new_index--;
            if(old_index != new_index) DanceRegCollections.Events.Move(old_index, new_index);
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
                Status.Set("Открываю " + ev.Title + "...", StatusString.LongTime);
                if (DanceRegCollections.LoadEvent(ev))
                {
                    DanceRegCollections.Active_events_windows.Value[ev.IdEvent].Show();
                }
                else DanceRegCollections.Active_events_windows.Value[ev.IdEvent].Activate();
            },
               (ev) => ev != null);
        }

        public static RelayCommand<DanceEvent> Command_DeleteEvent
        {
            get => new RelayCommand<DanceEvent>(ev =>
            {
                string text = "Вы уверены, что хотите удалить событие \"" + ev.Title + "\"?";

                if (DanceRegCollections.Active_events_windows.Value.ContainsKey(ev.IdEvent))
                {
                    text = "Cобытие \""+ ev.Title +"\" сейчас находится в работе, удаление невозможно!";
                    App.SetMessageBox(text, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
                else
                {
                    switch(App.SetMessageBox(text, System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question))
                    {
                        case System.Windows.MessageBoxResult.Yes:
                            DeleteEvent(ev);
                            break;
                    }
                }
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
 