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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.ViewModels
{
    public class EventManagerViewModel : ViewModel//, IDropTarget
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

        #region Search
        private TimerCallback Find_Callback;
        private Timer Find_Timer;

        private bool isShowNodes;
        /// <summary>
        /// Флаг для смены интерфейса узлов; true - показывать Nodes, false - Nominations
        /// </summary>
        public bool IsShowNodes
        {
            get => this.isShowNodes;
            private set
            {
                this.isShowNodes = value;
                if (value) this.OnPropertyChanged("Result_search_nodes");
                else this.OnPropertyChanged("Result_search_nomination");
                this.OnPropertyChanged("IsShowNodes");
            }
        }

        private string searchByNodeId;
        /// <summary>
        /// Поле для поиска по ID узла
        /// </summary>
        public string SearchByNodeId
        {
            get => this.searchByNodeId;
            set
            {
                if (this.Find_Timer != null) this.Find_Timer.Dispose();
                this.searchByNodeId = value;
                if (value != null && value.Length > 0) this.Find_Timer = new Timer(this.Find_Callback, 1, 500, 0);
                else this.ClearSearch();
                this.OnPropertyChanged("SearchByNodeId");
            }
        }

        private string searchByMemberNum;
        /// <summary>
        /// Поле для поиска по номеру участника
        /// </summary>
        public string SearchByMemberNum
        {
            get => this.searchByMemberNum;
            set
            {
                if (this.Find_Timer != null) this.Find_Timer.Dispose();
                this.searchByMemberNum = value;
                if (value != null && value.Length > 0) this.Find_Timer = new Timer(this.Find_Callback, 2, 500, 0);
                else this.ClearSearch();
                this.OnPropertyChanged("SearchByMemberNum");
            }
        }

        private string searchBySurname;
        public string SearchBySurname
        {
            get => this.searchBySurname;
            set
            {
                if (this.Find_Timer != null) this.Find_Timer.Dispose();
                this.searchBySurname = value;
                if (value != null && value.Length > 0) this.Find_Timer = new Timer(this.Find_Callback, 6, 500, 0);
                else this.ClearSearch();
                this.OnPropertyChanged("SearchBySurname");
            }
        }

        private JsonSchemeArray select_search_block;
        /// <summary>
        /// Выбранный блок в поиске
        /// </summary>
        public JsonSchemeArray Select_search_block
        {
            get => this.select_search_block;
            set
            {
                if (this.Find_Timer != null) this.Find_Timer.Dispose();
                this.select_search_block = value;
                if (value != null) this.Find_Timer = new Timer(this.Find_Callback, 3, 0, 5000);
                this.OnPropertyChanged("Select_search_block");
            }
        }

        private DanceNomination select_search_nomination;
        /// <summary>
        /// Выбранная номинация в поиске
        /// </summary>
        public DanceNomination Select_search_nomination
        {
            get => this.select_search_nomination;
            set
            {
                if (this.Find_Timer != null) this.Find_Timer.Dispose();
                this.select_search_nomination = value;
                if (value != null) this.Find_Timer = new Timer(this.Find_Callback, 4, 0, 5000);
                this.OnPropertyChanged("Select_search_nomination");
            }
        }

        private IdTitle select_search_school;
        /// <summary>
        /// Выбранная школа в поиске
        /// </summary>
        public IdTitle Select_search_school
        {
            get => this.select_search_school;
            set
            {
                if (this.Find_Timer != null) this.Find_Timer.Dispose();
                this.select_search_school = value;
                if (value != null) this.Find_Timer = new Timer(this.Find_Callback, 5, 0, 5000);
                this.OnPropertyChanged("Select_search_school");
            }
        }

        private int result_search_nodes_count;
        public int Result_search_nodes_count
        {
            get => this.result_search_nodes_count == -1 ? this.EventInWork.Nodes.Count : this.result_search_nodes_count;
            private set
            {
                this.result_search_nodes_count = value;
                this.OnPropertyChanged("Result_search_nodes_count");
            }
        }

        public ListExt<DanceNode> Result_search_nodes
        {
            get
            {
                ListExt<DanceNode> result = new ListExt<DanceNode>();
                if (this.SearchByNodeId != null && new Regex(@"^\d+$").IsMatch(this.SearchByNodeId))
                {
                    int search_node = Convert.ToInt32(this.SearchByNodeId);
                    foreach(DanceNode node in this.EventInWork.Nodes)
                    {
                        if(node.NodeId == search_node)
                        {
                            result.Add(node);
                            break;
                        }
                    }
                }
                else if(this.SearchByMemberNum != null && new Regex(@"^\d+$").IsMatch(this.SearchByMemberNum))
                {
                    int search_num = Convert.ToInt32(this.SearchByMemberNum);
                    foreach (DanceNode node in this.EventInWork.Nodes)
                    {
                        if (node.Member.MemberNum == search_num)
                        {
                            result.Add(node);
                        }
                    }
                }
                else if(this.Select_search_school != null)
                {
                    foreach (DanceNode node in this.EventInWork.Nodes)
                    {
                        if (node.Member.School == this.Select_search_school)
                        {
                            result.Add(node);
                        }
                    }
                }
                else if(this.SearchBySurname != null && this.SearchBySurname.Length > 0)
                {
                    foreach (DanceNode node in this.EventInWork.Nodes)
                    {
                        if (node.Member is MemberDancer dancer)
                        {
                            if (new Regex(this.SearchBySurname, RegexOptions.IgnoreCase).IsMatch(dancer.Surname))
                            {
                                result.Add(node);
                            }
                        }
                        else if(node.Member is MemberGroup group)
                        {
                            foreach(MemberDancer group_dancer in group.GroupMembers)
                            {
                                if (new Regex(this.SearchBySurname, RegexOptions.IgnoreCase).IsMatch(group_dancer.Surname))
                                {
                                    result.Add(node);
                                }
                            }
                        }
                    }
                }
                this.Result_search_nodes_count = result.Count;
                return result;
            }
        }

        private int result_search_nomination_count;
        public int Result_search_nomination_count
        {
            get => this.result_search_nomination_count == -1 ? this.EventInWork.Nominations.Count : this.result_search_nomination_count;
            private set
            {
                this.result_search_nomination_count = value;
                this.OnPropertyChanged("Result_search_nomination_count");
            }
        }


        public ListExt<DanceNomination> Result_search_nomination
        {
            get
            {
                ListExt<DanceNomination> result = null;

                if(this.Select_search_block != null)
                {
                    result = new ListExt<DanceNomination>();
                    foreach(DanceNomination nomination in this.EventInWork.Nominations)
                    {
                        if (nomination.Block_info.Id == this.Select_search_block.IdArray) result.Add(nomination);
                    }
                }
                else if(this.Select_search_nomination != null)
                {
                    result = new ListExt<DanceNomination>();
                    foreach (DanceNomination nomination in this.EventInWork.Nominations)
                    {
                        if (nomination.League_id == this.Select_search_nomination.League_id &&
                            nomination.Age_id == this.Select_search_nomination.Age_id &&
                            nomination.Style_id == this.Select_search_nomination.Style_id) result.Add(nomination);
                    }
                }
                this.Result_search_nomination_count = result == null ? -1 : result.Count;
                return result == null ? this.EventInWork.Nominations : result;
            }
        }

        private void StartSearchMethod(object obj)
        {
            if (this.Find_Timer != null) this.Find_Timer.Dispose();
            if(obj is int trigger_source)
            {
                switch (trigger_source)
                {
                    case 1: //node id
                        this.searchByMemberNum = "";
                        this.searchBySurname = "";
                        this.OnPropertyChanged("SearchBySurname");
                        this.OnPropertyChanged("SearchByMemberNum");
                        this.Select_search_block = null;
                        this.Select_search_nomination = null;
                        this.Select_search_school = null;
                        this.IsShowNodes = true;
                        break;
                    case 2: //num member
                        this.searchByNodeId = "";
                        this.searchBySurname = "";
                        this.OnPropertyChanged("SearchBySurname");
                        this.OnPropertyChanged("SearchByNodeId");
                        this.Select_search_block = null;
                        this.Select_search_nomination = null;
                        this.Select_search_school = null;
                        this.IsShowNodes = true;
                        break;
                    case 3: //block
                        this.searchByNodeId = "";
                        this.searchByMemberNum = "";
                        this.searchBySurname = "";
                        this.OnPropertyChanged("SearchBySurname");
                        this.OnPropertyChanged("SearchByMemberNum");
                        this.OnPropertyChanged("SearchByNodeId");
                        this.Select_search_nomination = null;
                        this.Select_search_school = null;
                        this.IsShowNodes = false;
                        break;
                    case 4: //nomination
                        this.searchByNodeId = "";
                        this.searchByMemberNum = "";
                        this.searchBySurname = "";
                        this.OnPropertyChanged("SearchBySurname");
                        this.OnPropertyChanged("SearchByMemberNum");
                        this.OnPropertyChanged("SearchByNodeId");
                        this.Select_search_block = null;
                        this.Select_search_school = null;
                        this.IsShowNodes = false;
                        break;
                    case 5: //school
                        this.searchByNodeId = "";
                        this.searchByMemberNum = "";
                        this.searchBySurname = "";
                        this.OnPropertyChanged("SearchBySurname");
                        this.OnPropertyChanged("SearchByMemberNum");
                        this.OnPropertyChanged("SearchByNodeId");
                        this.Select_search_block = null;
                        this.Select_search_nomination = null;
                        this.IsShowNodes = true;
                        break;
                    case 6:
                        this.searchByNodeId = "";
                        this.searchByMemberNum = "";
                        this.OnPropertyChanged("SearchByMemberNum");
                        this.OnPropertyChanged("SearchByNodeId");
                        this.Select_search_block = null;
                        this.Select_search_nomination = null;
                        this.Select_search_school = null;
                        this.IsShowNodes = true;
                        break;

                }
            }
        }

        /// <summary>
        /// Очистить поля поиска
        /// </summary>
        public void ClearSearch()
        {
            if (this.Find_Timer != null) this.Find_Timer.Dispose();
            this.searchByNodeId = "";
            this.searchByMemberNum = "";
            this.searchBySurname = "";
            this.OnPropertyChanged("SearchBySurname");
            this.OnPropertyChanged("SearchByMemberNum");
            this.OnPropertyChanged("SearchByNodeId");
            this.Select_search_block = null;
            this.Select_search_nomination = null;
            this.Select_search_school = null;
            this.IsShowNodes = false;
        }

        public RelayCommand Command_ClearSearch
        {
            get => new RelayCommand(obj =>
            {
                this.ClearSearch();
            });
        }
        #endregion

        public EventManagerViewModel(int idEventLoad)
        {
            this.EventInWork = DanceRegCollections.GetEventById(idEventLoad);
            this.EventInWork.Event_UpdateDanceEvent += UpdateEvent;
            this.EventInWork.Event_AddDeleteNodes += UpdateCountsForSearchList;
            this.EventInWork.Command_AddDancerUseMember = this.Command_AddDancerUseMember;
            this.EventInWork.Command_AddDancerUseNomination = this.Command_AddDancerUseNomination;
            this.EventInWork.Command_deleteNode = this.Command_DeleteNode;
            this.Find_Callback = new TimerCallback(this.StartSearchMethod);
            this.eventEditTitle = this.EventInWork.Title;
            this.startDateEvent = UnixTime.ToDateTimeOffset(this.EventInWork.StartEventTimestamp, App.Locality).DateTime;
            this.Title = "Менеджер событий - " + App.AppTitle;
            this.TitleUpdate_Callback = new TimerCallback(this.UpdateEventTitleMethod);
            this.Initialize();
            this.ClearSearch();
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

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from nominations where Id_event=" + this.EventInWork.IdEvent);
            Dictionary<int, JsonSchemeArray> tmp_blocks = new Dictionary<int, JsonSchemeArray>();
            foreach(DbRow row in res)
            {
                int id_block = row["Id_block"].ToInt32();
                string title_block = "";
                JudgeType type_block = JudgeType.ThreeD;
                if (!tmp_blocks.ContainsKey(id_block))
                {
                    tmp_blocks.Add(id_block, this.EventInWork.SchemeEvent.GetSchemeArrayById(id_block, SchemeType.Block));
                }
                title_block = tmp_blocks[id_block].Title;
                type_block = tmp_blocks[id_block].ScoreType;

                this.EventInWork.AddNomination(new DanceNomination(this.EventInWork.IdEvent, new IdTitle(id_block, title_block), type_block, row["Id_league"].ToInt32(), row["Id_age"].ToInt32(), row["Id_style"].ToInt32(), row["Is_show_in_search"].ToBoolean(), row["Json_judge_ignore"].ToString()));
            }

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
                    //DbResult res_member = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from " + (isGroup ? "groups" : "dancers") + " where Id_member=" + row["Id_member"].ToInt32());
                    string table_name = (isGroup ? "groups" : "dancers");
                    int id_member = row["Id_member"].ToInt32();
                    DbResult res_member = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from "+ table_name + " left join nums_for_members on (nums_for_members.Id_event="+ this.EventInWork.IdEvent +" and nums_for_members.Id_member="+ id_member + " and nums_for_members.Is_group="+ isGroup +") where "+ table_name + ".Id_member="+ id_member + ";");

                    if (!isGroup)
                    {
                        tmp_member = new MemberDancer(this.EventInWork.IdEvent, res_member["Id_member", 0].ToInt32(), res_member["Firstname", 0].ToString(), res_member["Surname", 0].ToString());
                    }
                    else
                    {
                        tmp_member = new MemberGroup(this.EventInWork.IdEvent, res_member["Id_member", 0].ToInt32(), res_member["Json_members", 0].ToString());
                        
                    }
                    tmp_member.MemberNum = res_member["Number", 0].Value == DBNull.Value ? 0 : res_member["Number", 0].ToInt32();
                    tmp_member.SetSchool(DanceRegCollections.GetSchoolById(res_member["Id_school", 0].ToInt32()));
                    await this.EventInWork.AddMember(tmp_member);
                }

                //add node
                IdTitle tmp_platform = new IdTitle(row["Id_platform"].ToInt32(), this.EventInWork.SchemeEvent.GetTypeTitleById(row["Id_platform"].ToInt32(), SchemeType.Platform));
                IdTitle tmp_block = new IdTitle(row["Id_block"].ToInt32(), this.EventInWork.SchemeEvent.GetTypeTitleById(row["Id_block"].ToInt32(), SchemeType.Block));

                this.EventInWork.AddNode(row["Id_node"].ToInt32(), tmp_member, isGroup, tmp_platform, row["Id_league"].ToInt32(), tmp_block, row["Id_age"].ToInt32(), row["Id_style"].ToInt32(), row["Json_scores"].ToString(), row["Position"].ToInt32());
            }
        }

        private void UpdateCountsForSearchList()
        {
            if (this.IsShowNodes)
            {
                this.OnPropertyChanged("Result_search_nodes_count");
                this.OnPropertyChanged("Result_search_nodes");
            }
            else
            {
                //if (this.Select_search_block != null || this.Select_search_nomination != null)
                //{
                    this.OnPropertyChanged("Result_search_nomination");
                //}
                this.OnPropertyChanged("Result_search_nomination_count");
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
            this.EventInWork.Event_AddDeleteNodes -= UpdateCountsForSearchList;
            this.EventInWork.Command_deleteNode = null;
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
        /*
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
        */
        private async void DeleteNodeMethod(DanceNode deleteNode)
        {
            await this.EventInWork.DeleteNodeAsync(deleteNode);
        }

        private async void SetRandomNumsMethod()
        {
            await this.EventInWork.SetRandomNums();

            int step = 0;
            foreach(DanceNomination nomination in this.EventInWork.Nominations)
            {
                nomination.SortByNums(step);
                step += nomination.Nominants.Count;
            }
            await this.EventInWork.UpdateNodePosition(0, this.EventInWork.Nodes.Count - 1);
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

        public RelayCommand Command_SetRandomNums
        {
            get => new RelayCommand(obj =>
            {
                this.SetRandomNumsMethod();
            });
        }

        public RelayCommand<MemberDancer> Command_AddDancerUseMember
        {
            get => new RelayCommand<MemberDancer>(dancer =>
            {
                AddDancerView window = new AddDancerView(this.EventInWork.IdEvent, dancer);
                window.ShowDialog();
            });
        }

        public RelayCommand<DanceNomination> Command_AddDancerUseNomination
        {
            get => new RelayCommand<DanceNomination>(nomination =>
            {
                AddDancerView window = new AddDancerView(nomination);
                window.ShowDialog();
            });
        }
    }
}
