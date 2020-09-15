using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Views.EventManagerViews;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.ViewModels
{
    public class EventManagerViewModel : ViewModel, IDropTarget
    {
        private TimerCallback TitleUpdate_Callback;
        private Timer TitleUpdate_Timer;

        private DanceNode select_node;
        public DanceNode Select_node
        {
            get => this.select_node;
            set
            {
                this.select_node = value;
                this.OnPropertyChanged("Select_node");
            }
        }

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

            await DanceRegCollections.LoadSchools();

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from event_nodes where Id_event=" + this.EventInWork.IdEvent + " order by Position");
            foreach(DbRow row in res)
            {
                //add member
                Member tmp_member = null;
                bool isGroup = row["Is_group"].ToBoolean();
                if (!isGroup)
                {
                    tmp_member = this.EventInWork.GetDancerById(row["Id_member"].ToInt32());
                }
                else tmp_member = this.EventInWork.GetGroupById(row["Id_member"].ToInt32());

                if(tmp_member == null)
                {
                    DbResult res_member = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from " + (isGroup ? "groups" : "dancers") + " where Id_member=" + row["Id_member"].ToInt32());

                    if (!isGroup)
                    {
                        tmp_member = new MemberDancer(this.EventInWork.IdEvent, res_member["Id_member", 0].ToInt32(), res_member["Firstname", 0].ToString(), res_member["Surname", 0].ToString());
                    }
                    else
                    {
                        tmp_member = new MemberGroup(this.EventInWork.IdEvent, res_member["Id_member", 0].ToInt32(), res_member["Json_members", 0].ToString());
                        
                    }
                    tmp_member.SetSchool(DanceRegCollections.GetSchoolById(res_member["Id_school", 0].ToInt32()));
                    this.EventInWork.AddMember(tmp_member);
                }

                //add node
                IdTitle tmp_platform = new IdTitle(row["Id_platform"].ToInt32(), this.EventInWork.SchemeEvent.GetTypeTitleById(row["Id_platform"].ToInt32(), SchemeType.Platform));
                IdTitle tmp_block = new IdTitle(row["Id_block"].ToInt32(), this.EventInWork.SchemeEvent.GetTypeTitleById(row["Id_block"].ToInt32(), SchemeType.Block));

                this.EventInWork.AddNode(row["Id_node"].ToInt32(), tmp_member, isGroup, tmp_platform, row["Id_league"].ToInt32(), tmp_block, row["Id_age"].ToInt32(), row["Id_style"].ToInt32(), row["Json_scores"].ToString());
            }
        }

        private void UpdateEventTitleMethod(object obj)
        {
            if (this.TitleUpdate_Timer != null) this.TitleUpdate_Timer.Dispose();
            this.EventInWork.SetTitle(this.EventEditTitle);
        }

        public override WindowClose CloseMethod()
        {
            this.EventInWork.All_members_count = this.EventInWork.Dancers.Count + this.EventInWork.Groups.Count;
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

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public async void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DanceNode node)
            {
                int oldIndex = this.EventInWork.Nodes.IndexOf(node);

                int insert_index = dropInfo.InsertIndex;
                if (insert_index > oldIndex) insert_index -= 1;
                DanceNode new_node = this.EventInWork.Nodes[insert_index];
                int newIndex = this.EventInWork.Nodes.IndexOf(new_node);



                this.EventInWork.Nodes.Move(oldIndex, newIndex);
                //this.OnPropertyChanged("EventInWork");
                await this.EventInWork.UpdateNodePosition(oldIndex, newIndex);

            }
        }

        private async void DeleteNodeMethod(DanceNode deleteNode)
        {
            await this.EventInWork.DeleteNodeAsync(deleteNode);
        }

        public RelayCommand Command_AddDancer
        {
            get => new RelayCommand(obj =>
            {
                AddDancerView window = new AddDancerView(this.EventInWork.IdEvent);
                window.ShowDialog();
            });
        }

        public RelayCommand Command_AddGroup
        {
            get => new RelayCommand(obj =>
            {
                AddGroupView window = new AddGroupView(this.EventInWork.IdEvent);
                window.ShowDialog();
            });
        }

        public RelayCommand Command_EditPlatforms
        {
            get => new RelayCommand(obj =>
            {
                PlatformsManagerView window = new PlatformsManagerView(this.EventInWork.IdEvent);
                if ((bool)window.ShowDialog())
                {
                    this.OnPropertyChanged("Nodes");
                }
            });
        }

        public RelayCommand Command_EditJudge
        {
            get => new RelayCommand(obj =>
            {
                JudgeManagerView window = new JudgeManagerView(this.EventInWork.IdEvent);
                window.ShowDialog();
            });
        }

        public RelayCommand<DanceNode> Command_DeleteNode
        {
            get => new RelayCommand<DanceNode>(deleteNode =>
            {
                this.DeleteNodeMethod(deleteNode);
            },
                (deleteNode) => deleteNode != null );
        }
    }
}
