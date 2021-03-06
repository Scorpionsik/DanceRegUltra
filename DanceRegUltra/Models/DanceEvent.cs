﻿using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace DanceRegUltra.Models
{
    public delegate void UpdateDanceEvent(int event_id, string column_name, object value_update);

    public class DanceEvent : NotifyPropertyChanged, IComparable<DanceEvent>
    {
        private int nodeId;
        public int NodeId
        {
            get => this.nodeId;
            private set
            {
                this.nodeId = value;
                this.OnPropertyChanged("NodeId");
                this.event_updateDanceEvent?.Invoke(this.IdEvent, "Id_node_increment", value);
            }
        }

        private int all_members_count;
        public int All_members_count
        {
            get => this.all_members_count;
            set
            {
                this.all_members_count = value;
                this.OnPropertyChanged("All_members_count");
                this.event_updateDanceEvent?.Invoke(this.IdEvent, "All_member_count", value);
            }
        }

        private bool enableRandNums;
        public bool EnableRandNums
        {
            get => this.enableRandNums;
            private set
            {
                this.enableRandNums = value;
                this.OnPropertyChanged("EnableRandNums");
                this.event_updateDanceEvent?.Invoke(this.IdEvent, "Enable_rand_nums", value);
            }
        }
        /*
        private int member_finish_count;
        public int Member_finish_count
        {
            get => this.member_finish_count;
            set
            {
                this.member_finish_count = value;
                this.OnPropertyChanged("Member_finish_count");
                this.OnPropertyChanged("Member_finish_percent");
                this.event_updateDanceEvent?.Invoke(this.IdEvent, "Member_finish_count", value);
            }
        }

        public string Member_finish_percent
        {
            get
            {
                if (this.HideNodes.Value.Count == 0) return "0";
                else
                {
                    double percent = this.HideNodes.Value.Count / 100 * this.Member_finish_count;
                    string str_percent = percent.ToString();
                    if (str_percent.Length > 4) str_percent = str_percent.Remove(3);
                    return str_percent;
                }
            }
        }*/

        private event UpdateDanceEvent event_updateDanceEvent;
        /// <summary>
        /// Срабатывает при изменении значений события; передает в числовой переменной id данного события, в строке название столбца в таблице бд
        /// </summary>
        public event UpdateDanceEvent Event_UpdateDanceEvent
        {
            add
            {
                this.event_updateDanceEvent -= value;
                this.event_updateDanceEvent += value;
            }
            remove => this.event_updateDanceEvent -= value;
        }

        private event Action event_AddDeleteNodes;
        public event Action Event_AddDeleteNodes
        {
            add
            {
                this.event_AddDeleteNodes -= value;
                this.event_AddDeleteNodes += value;
            }
            remove => this.event_AddDeleteNodes -= value;
        }

        private event Action<DanceEvent> event_UpdateTimeDate;
        public event Action<DanceEvent> Event_UpdateTimeDate
        {
            add
            {
                this.event_UpdateTimeDate -= value;
                this.event_UpdateTimeDate += value;
            }
            remove => this.event_UpdateTimeDate -= value;
        }

        private Lazy<ListExt<MemberDancer>> HideDancers;
        public ListExt<MemberDancer> Dancers { get => this.HideDancers?.Value; }

        private Lazy<ListExt<MemberGroup>> HideGroups;
        public ListExt<MemberGroup> Groups { get => this.HideGroups?.Value; }

        private Lazy<ListExt<DanceNode>> HideNodes;
        public ListExt<DanceNode> Nodes { get => this.HideNodes?.Value; }

        private Lazy<ListExt<DanceNomination>> HideNominations;
        public ListExt<DanceNomination> Nominations { get => this.HideNominations?.Value; }

        public int IdEvent { get; private set; }

        private string title;
        public string Title
        {
            get => this.title;
            private set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }

        private double startEventTimestamp;
        public double StartEventTimestamp
        {
            get => this.startEventTimestamp;
            private set
            {
                this.startEventTimestamp = value;
                this.event_UpdateTimeDate?.Invoke(this);
                this.OnPropertyChanged("StartEventTimestamp");
            }
        }

        public double EndEventTimestamp { get; private set; }

        public JsonScheme SchemeEvent { get; private set; }

        public Dictionary<int, List<IdTitle>> Leagues { get; private set; }
        public Dictionary<int, List<IdTitle>> Ages { get; private set; }
        public List<int> Styles { get; private set; }

        public ListExt<IdTitle> Schools { get; private set; }

        //public string JsonSchemeEvent { get; private set; }

        public DanceEvent(int id, string title, double startTimestamp, double endTimestamp, string json = "", int node_id = 1)
        {
            this.NodeId = node_id;

            this.HideDancers = new Lazy<ListExt<MemberDancer>>();
            this.HideGroups = new Lazy<ListExt<MemberGroup>>();
            this.HideNodes = new Lazy<ListExt<DanceNode>>();
            this.HideNominations = new Lazy<ListExt<DanceNomination>>();
            this.Schools = new ListExt<IdTitle>();
            this.IdEvent = id;
            this.title = title;
            this.startEventTimestamp = startTimestamp;
            this.EndEventTimestamp = endTimestamp;
            //this.JsonSchemeEvent = json;
            if (json != null && json.Length > 0) this.SchemeEvent = JsonScheme.Deserialize(json);
            this.Leagues = new Dictionary<int, List<IdTitle>>();
            this.Ages = new Dictionary<int, List<IdTitle>>();
            this.Styles = new List<int>();

            this.event_updateDanceEvent = null;

            this.Command_EditEvent = MainViewModel.Command_EditEvent;
            this.Command_DeleteEvent = MainViewModel.Command_DeleteEvent;
        }

        public void UnloadEvent()
        {
            this.HideDancers = new Lazy<ListExt<MemberDancer>>();
            this.HideGroups = new Lazy<ListExt<MemberGroup>>();
            this.HideNodes = new Lazy<ListExt<DanceNode>>();
            this.HideNominations = new Lazy<ListExt<DanceNomination>>();
            this.Schools = new ListExt<IdTitle>();

            this.Leagues = new Dictionary<int, List<IdTitle>>();
            this.Ages = new Dictionary<int, List<IdTitle>>();
            this.Styles = new List<int>();
        }

        public void SetEnableRandNums(bool value)
        {
            this.EnableRandNums = value;
        }

        private async void AddNominationMember(DanceNode node)
        {
            DanceNomination tmp_nomination = null;
            foreach(DanceNomination nomination in this.HideNominations.Value)
            {
                if(nomination.League_id == node.LeagueId &&
                    nomination.Age_id == node.AgeId &&
                    nomination.Block_info.Id == node.Block.Id &&
                    nomination.Style_id == node.StyleId)
                {
                    tmp_nomination = nomination;
                    break;
                }
            }
            if(tmp_nomination == null)
            {
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from nominations where Id_event=" + this.IdEvent + " and Id_league=" + node.LeagueId + " and Id_age=" + node.AgeId + " and Id_style=" + node.StyleId);

                await DanceRegDatabase.ExecuteNonQueryAsync("insert into nominations values (" + this.IdEvent + ", " + node.Block.Id + ", " + node.LeagueId + ", " + node.AgeId + ", " + node.StyleId +", "+ !res.HasRows +",'"+ JsonConvert.SerializeObject(new List<bool>() { false, false, false, false }) +"', 1)");

                JsonSchemeArray tmp_block = this.SchemeEvent.GetSchemeArrayById(node.Block.Id, Enums.SchemeType.Block);
                DanceNomination nomination = new DanceNomination(this.IdEvent, node.Block, tmp_block.ScoreType, node.LeagueId, node.AgeId, node.StyleId, !res.HasRows);
                nomination.SetJudgeCount(tmp_block.JudgeCount);
                this.AddNomination(nomination);
                tmp_nomination = nomination;
            }
            tmp_nomination.AddNominant(node);
            this.event_AddDeleteNodes?.Invoke();
        }

        public void AddNomination(DanceNomination nomination)
        {
            nomination.Event_UpdateNominant += this.UpdateNominantPosition;
            nomination.Command_AddDancerUseNomination = this.Command_AddDancerUseNomination;
            nomination.Command_AddGroupUseNomination = this.Command_AddGroupUseNomination;
            nomination.Command_DeleteNodesByNomination = this.Command_DeleteNodesByNomination;
            nomination.Command_editSelectNomination = this.Command_editSelectNomination;
            nomination.Command_ChangeBlockForNomination = this.Command_ChangeBlockForNomination;
            nomination.Command_NodeSetScore = this.Command_SetScore;
            int index = 0;
            while (index < this.HideNominations.Value.Count && this.SchemeEvent.Compare(this.HideNominations.Value[index], nomination) != 1) index++;

            //nomination.SetNewType(tmp_block.ScoreType);

            this.HideNominations.Value.Insert(index, nomination);
            this.OnPropertyChanged("Nominations");
        }

        public void DeleteNomination(int block_id, int league_id, int age_id, int style_id)
        {
            /*
            int index = 0;
            while (index < this.HideNominations.Value.Count &&
                this.HideNominations.Value[index].Block_info.Id != block_id &&
                this.HideNominations.Value[index].League_id != league_id &&
                this.HideNominations.Value[index].Age_id != age_id &&
                this.HideNominations.Value[index].Style_id != style_id) index++;
                */

            for (int index = 0; index < this.HideNominations.Value.Count; index++)
            {
                if (this.HideNominations.Value[index].Block_info.Id == block_id &&
                this.HideNominations.Value[index].League_id == league_id &&
                this.HideNominations.Value[index].Age_id == age_id &&
                this.HideNominations.Value[index].Style_id == style_id)
                {
                    this.HideNominations.Value[index].Event_UpdateNominant -= this.UpdateNominantPosition;
                    this.HideNominations.Value[index].Command_AddDancerUseNomination = null;
                    this.HideNominations.Value[index].Command_AddGroupUseNomination = null;
                    this.HideNominations.Value[index].Command_DeleteNodesByNomination = null;
                    this.HideNominations.Value[index].Command_editSelectNomination = null;
                    this.HideNominations.Value[index].Command_ChangeBlockForNomination = null;
                    this.HideNominations.Value[index].Command_NodeSetScore = null;
                    this.HideNominations.Value.RemoveAt(index);
                    break;
                }
            }
            
        }

        private async void UpdateNominantPosition(DanceNode node, int old_index, int new_index)
        {
            if (old_index != new_index)
            {

                int true_old_index = this.HideNodes.Value.IndexOf(node);
                int true_new_index = true_old_index + (new_index - old_index);
                this.HideNodes.Value.Move(true_old_index, true_new_index);
                await this.UpdateNodePosition(true_old_index < true_new_index ? true_old_index : true_new_index, this.HideNodes.Value.Count - 1, true);
            }
        }

        private async void UpdateDanceNode(int event_id, int node_id, string column_name, object value)
        {
            await DanceRegDatabase.ExecuteNonQueryAsync("update event_nodes set " + column_name + "='" + value + "' where Id_event=" + event_id + " and Id_node=" + node_id);
        }

        public void AddNode(int node_id, Member member, bool isGroup, IdTitle platform, int league_id, IdTitle block, int age_id, int style_id, string scores, int position, int prize)
        {
            DanceNode newNode = new DanceNode(this.IdEvent, node_id, member, isGroup, platform, league_id, block, age_id, style_id);
            newNode.Command_deleteNode = this.Command_deleteNode;
            newNode.Command_ChangeBlockForNode = this.Command_ChangeBlockForNode;
            newNode.Command_SetScore = this.Command_SetScore;
            newNode.Position = position;
            newNode.Event_UpdateDanceNode += this.UpdateDanceNode;
            newNode.SetScores(scores);
            newNode.SetPrizePlace(prize);
            this.HideNodes.Value.Add(newNode);
            this.AddNominationMember(newNode);
            this.AddSchool(member.School);
        }

        private void AddSchool(IdTitle school)
        {
            if (!this.Schools.Contains(school))
            {
                int index = 0;
                while (index < this.Schools.Count && this.Schools[index].CompareTo(school) != 1) index++;
                this.Schools.Insert(index, school);
                this.OnPropertyChanged("Schools");
            }
        }

        private async void RemoveSchool(IdTitle school)
        {
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select event_nodes.Id_node from event_nodes left join dancers on (event_nodes.Id_member=dancers.Id_member) left join groups on (event_nodes.Id_member=groups.Id_member) where event_nodes.Id_event="+ this.IdEvent +" and dancers.Id_school="+ school.Id);
            if (!res.HasRows)
            {
                this.Schools.Remove(school);
                this.OnPropertyChanged("Schools");
            }
        }

        public async Task<int> AddNodeAsync(Member member, bool isGroup, IdTitle platform, int league_id, IdTitle block, int age_id, int style_id)
        {
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from event_nodes where Id_event=" + this.IdEvent + " and Id_member=" + member.MemberId + " and Is_group=" + isGroup + " and Id_platform=" + platform.Id + " and Id_league=" + league_id + " and Id_block=" + block.Id + " and Id_age=" + age_id + " and Id_style=" + style_id);
            if (res.RowsCount == 0)
            {
                DanceNode newNode = new DanceNode(this.IdEvent, this.NodeId++, member, isGroup, platform, league_id, block, age_id, style_id);
                newNode.Event_UpdateDanceNode += this.UpdateDanceNode;
                bool getPosition = false;
                int position = 0;
                for (; position < this.HideNodes.Value.Count; position++)
                {
                    if (this.SchemeEvent.Compare(this.HideNodes.Value[position], newNode) == 1)
                    {
                        this.HideNodes.Value.Insert(position, newNode);
                        
                        getPosition = true;
                        break;
                    }
                }

                if (!getPosition) this.HideNodes.Value.Add(newNode);
                newNode.Position = position;
                newNode.Command_deleteNode = this.Command_deleteNode;
                newNode.Command_ChangeBlockForNode = this.Command_ChangeBlockForNode;
                this.AddNominationMember(newNode);
                this.AddSchool(newNode.Member.School);
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into event_nodes values (" + this.IdEvent + ", " + newNode.NodeId + ", " + newNode.Member.MemberId + ", " + isGroup + ", " + newNode.Platform.Id + ", " + newNode.LeagueId + ", " + newNode.Block.Id + ", " + newNode.AgeId + ", " + newNode.StyleId + ", '', 0, 0, " + position + ")");
                //if (position < this.HideNodes.Value.Count - 1) await this.UpdateNodePosition(position, this.HideNodes.Value.Count - 1);
                return position;
                //this.HideNodes.Value.Add(newNode);
            }
            else return -1;
        }

        public async Task<int> DeleteNodeAsync(DanceNode node, bool move_node = false)
        {
            int position = this.HideNodes.Value.IndexOf(node);
            if (position > -1)
            {
                //if (node.IsPrintPrice) this.Member_finish_count--;
                node.Event_UpdateDanceNode -= this.UpdateDanceNode;
                node.Command_ChangeBlockForNode = null;
                node.Command_deleteNode = null;
                this.HideNodes.Value.RemoveAt(position);
                await DanceRegDatabase.ExecuteNonQueryAsync("delete from event_nodes where Id_event=" + this.IdEvent + " and Id_node=" + node.NodeId);
                //if (position < this.HideNodes.Value.Count - 1) await this.UpdateNodePosition(position, this.HideNodes.Value.Count - 1, true);
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from event_nodes where Id_event=" + this.IdEvent + " and Id_member=" + node.Member.MemberId + " and Is_group=" + node.IsGroup);
                if (res.RowsCount == 0 && !move_node)
                {
                    if (node.IsGroup)
                    {
                        this.HideGroups.Value.Remove((MemberGroup)node.Member);
                    }
                    else this.HideDancers.Value.Remove((MemberDancer)node.Member);
                    
                    //this.All_members_count--;
                }

                res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from event_nodes where Id_event=" + this.IdEvent + " and Id_block=" + node.Block.Id + " and Id_league=" + node.LeagueId + " and Id_age=" + node.AgeId + " and Id_style=" + node.StyleId);
                if (res.RowsCount == 0)
                {
                    await DanceRegDatabase.ExecuteNonQueryAsync("delete from nominations where Id_event=" + this.IdEvent + " and Id_block=" + node.Block.Id + " and Id_league=" + node.LeagueId + " and Id_age=" + node.AgeId + " and Id_style=" + node.StyleId);
                    this.DeleteNomination(node.Block.Id, node.LeagueId, node.AgeId, node.StyleId);
                }
                else
                {
                    foreach(DanceNomination nomination in this.HideNominations.Value)
                    {
                        if (
                            node.Block.Id == nomination.Block_info.Id &&
                            node.LeagueId == nomination.League_id &&
                            node.AgeId == nomination.Age_id &&
                            node.StyleId == nomination.Style_id)
                        {
                            nomination.Nominants.Remove(node);
                            break;
                        }
                    }
                }

                this.RemoveSchool(node.Member.School);
                this.event_AddDeleteNodes?.Invoke();
            }
            return position;
        }

        public async Task UpdateNodePosition(int index1, int index2, bool isUnknownPosition)
        {
            int[] minmax = new int[2] { Math.Min(index1, index2), Math.Max(index1, index2) };

            await DanceRegDatabase.ManualOpen("UpdateNodePosition");
            for (int i = minmax[0]; i <= minmax[1]; i++)
            {
                //DanceRegCollections.Ages.Value[i].Position = i + 1;
                if(isUnknownPosition) this.Nodes[i].Position = i;
                await DanceRegDatabase.ExecuteNonQueryAsync("update event_nodes set Position=" + (isUnknownPosition ? i : this.Nodes[i].Position) + " where Id_event=" + this.IdEvent + " and Id_node=" + this.Nodes[i].NodeId);
            }
            DanceRegDatabase.ManualClose("UpdateNodePosition");
        }

        public async Task AddMember(Member newMember)
        {
            int index_sort = 0;
            bool isGroup = false;
            if (newMember is MemberDancer dancer)
            {
                /*
                MemberDancer tmp_add = null;
                if (dancer.EventId != this.IdEvent)
                {
                    tmp_add = new MemberDancer(this.IdEvent, dancer.MemberId, dancer.Name, dancer.Surname);
                    tmp_add.SetSchool(dancer.School);
                }
                else tmp_add = dancer;
                */
                dancer.Command_AddDancerUseMember = this.Command_AddDancerUseMember;
                dancer.Command_EditDancer = this.Command_EditDancer;
                while (index_sort < this.HideDancers.Value.Count && this.HideDancers.Value[index_sort].CompareTo(dancer) <= 0) index_sort++;
                //if (index_sort > this.HideDancers.Value.Count) index_sort--;
                this.HideDancers.Value.Insert(index_sort, dancer);
                this.OnPropertyChanged("Dancers");
            }
            else if(newMember is MemberGroup group)
            {
                isGroup = true;
                /*
                MemberGroup tmp_add = null;
                if (group.EventId != this.IdEvent)
                {
                    tmp_add = new MemberGroup(this.IdEvent, group.MemberId, group.GroupMembers);
                }
                else tmp_add = group;
                */
                group.Command_AddGroupUseMember = this.Command_AddGroupUseMember;
                group.Command_EditGroup = this.Command_EditGroup;
                while (index_sort < this.HideGroups.Value.Count && this.HideGroups.Value[index_sort].CompareTo(group) <= 0) index_sort++;
                //if (index_sort > this.HideGroups.Value.Count) index_sort--;
                this.HideGroups.Value.Insert(index_sort, group);
                this.OnPropertyChanged("Groups");
            }
            newMember.Command_DeleteNodesByMember = this.Command_DeleteNodesByMember;
            if (newMember.MemberNum == 0)
            {
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Num_increment from events where Id_event=" + this.IdEvent);
                newMember.MemberNum = res.GetInt32("Num_increment", 0);
                this.event_updateDanceEvent?.Invoke(this.IdEvent, "Num_increment", newMember.MemberNum + 1);
                //await DanceRegDatabase.ExecuteNonQueryAsync("update events set Num_increment=" + (newMember.MemberNum + 1) + " Id_event=" + newMember.EventId);
                await this.UpdateMemberNum(newMember.MemberId, isGroup, newMember.MemberNum);
            }
            //this.All_members_count++;
        }

        public MemberDancer GetDancerById(int id_dancer)
        {
            foreach(MemberDancer dancer in this.HideDancers.Value)
            {
                if (dancer.MemberId == id_dancer) return dancer;
            }
            return null;
        }

        public MemberGroup GetGroupById(int id_group)
        {
            foreach(MemberGroup group in this.HideGroups.Value)
            {
                if (group.MemberId == id_group) return group;
            }
            return null;
        }

        public DanceNomination GetNominationByNode(DanceNode node)
        {
            foreach(DanceNomination nomination in this.Nominations)
            {
                if (nomination.Nominants.Contains(node)) return nomination;
            }
            return null;
        }

        public DanceNomination GetNominationBySchemeInfo(int league_id, int block_id, int age_id, int style_id)
        {
            foreach (DanceNomination nomination in this.Nominations)
            {
                if(
                    nomination.League_id == league_id &&
                    nomination.Block_info.Id == block_id &&
                    nomination.Age_id == age_id &&
                    nomination.Style_id == style_id
                    )
                {
                    return nomination;
                }
            }
            return null;
        }
        
        public void SetTitle(string newTitle)
        {
            this.Title = newTitle;
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "Title", newTitle);
        }

        public void SetStartTimestampEvent(double newTimestamp)
        {
            this.StartEventTimestamp = newTimestamp;
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "Start_timestamp", newTimestamp);
        }

        public void SetJsonScheme(string newJson)
        {
            //this.JsonSchemeEvent = newJson;
            this.SchemeEvent = JsonScheme.Deserialize(newJson);
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "Json_scheme", newJson);
        }

        public void SetEndTimestampEvent()
        {
            this.EndEventTimestamp = UnixTime.CurrentUnixTimestamp();
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "End_timestamp", this.EndEventTimestamp);
        }

        public RelayCommand<DanceEvent> Command_EditEvent { get; private set; }
        public RelayCommand<DanceEvent> Command_DeleteEvent { get; private set; }

        public int CompareTo(DanceEvent obj)
        {
            double res = this.StartEventTimestamp - obj.StartEventTimestamp;

            if (res == 0) return 0;
            else return res < 0 ? -1 : 1;
        }
        //---метод OnPropertyChanged

        public async Task SetRandomNums()
        {
            ListExt<Member> result = new ListExt<Member>();
            result.AddRange(this.HideDancers.Value);
            result.AddRange(this.HideGroups.Value);
            result = result.Shuffle();

            int num = 100;
            foreach (Member member in result)
            {
                member.MemberNum = num++;
                bool isGroup = member is MemberDancer ? false : true;
                await this.UpdateMemberNum(member.MemberId, isGroup, member.MemberNum);
            }


        }

        private RelayCommand<DanceNode> command_deleteNode;
        public RelayCommand<DanceNode> Command_deleteNode
        {
            get => this.command_deleteNode;
            set
            {
                this.command_deleteNode = value;
                this.OnPropertyChanged("Command_deleteNode");
            }
        }

        private RelayCommand<MemberDancer> command_AddDancerUseMember;
        public RelayCommand<MemberDancer> Command_AddDancerUseMember
        {
            get => this.command_AddDancerUseMember;
            set
            {
                this.command_AddDancerUseMember = value;
                this.OnPropertyChanged("Command_AddDancerUseMember");
            }
        }

        private RelayCommand<DanceNomination> command_AddDancerUseNomination;
        public RelayCommand<DanceNomination> Command_AddDancerUseNomination
        {
            get => this.command_AddDancerUseNomination;
            set
            {
                this.command_AddDancerUseNomination = value;
                this.OnPropertyChanged("Command_AddDancerUseNomination");
            }
        }

        private RelayCommand<MemberGroup> command_AddGroupUseMember;
        public RelayCommand<MemberGroup> Command_AddGroupUseMember
        {
            get => this.command_AddGroupUseMember;
            set
            {
                this.command_AddGroupUseMember = value;
                this.OnPropertyChanged("Command_AddGroupUseMember");
            }
        }

        private RelayCommand<DanceNomination> command_AddGroupUseNomination;
        public RelayCommand<DanceNomination> Command_AddGroupUseNomination
        {
            get => this.command_AddGroupUseNomination;
            set
            {
                this.command_AddGroupUseNomination = value;
                this.OnPropertyChanged("Command_AddGroupUseNomination");
            }
        }

        private RelayCommand<Member> command_DeleteNodesByMember;
        public RelayCommand<Member> Command_DeleteNodesByMember
        {
            get => this.command_DeleteNodesByMember;
            set
            {
                this.command_DeleteNodesByMember = value;
                this.OnPropertyChanged("Command_DeleteNodesByMember");
            }
        }

        private RelayCommand<DanceNomination> command_DeleteNodesByNomination;
        public RelayCommand<DanceNomination> Command_DeleteNodesByNomination
        {
            get => this.command_DeleteNodesByNomination;
            set
            {
                this.command_DeleteNodesByNomination = value;
                this.OnPropertyChanged("Command_DeleteNodesByNomination");
            }
        }

        private RelayCommand<DanceNomination> command_editSelectNomination;
        public RelayCommand<DanceNomination> Command_editSelectNomination
        {
            get => this.command_editSelectNomination;
            set
            {
                this.command_editSelectNomination = value;
                this.OnPropertyChanged("Command_editSelectNomination");
            }
        }

        private RelayCommand<MemberDancer> command_EditDancer;
        public RelayCommand<MemberDancer> Command_EditDancer
        {
            get => this.command_EditDancer;
            set
            {
                this.command_EditDancer = value;
                this.OnPropertyChanged("Command_EditDancer");
            }
        }

        private RelayCommand<MemberGroup> command_EditGroup;
        public RelayCommand<MemberGroup> Command_EditGroup
        {
            get => this.command_EditGroup;
            set
            {
                this.command_EditGroup = value;
                this.OnPropertyChanged("Command_EditGroup");
            }
        }

        private RelayCommand<DanceNode> command_ChangeBlockForNode;
        public RelayCommand<DanceNode> Command_ChangeBlockForNode
        {
            get => this.command_ChangeBlockForNode;
            set
            {
                this.command_ChangeBlockForNode = value;
                this.OnPropertyChanged("Command_ChangeBlockForNode");
            }
        }

        private RelayCommand<DanceNomination> command_ChangeBlockForNomination;
        public RelayCommand<DanceNomination> Command_ChangeBlockForNomination
        {
            get => this.command_ChangeBlockForNomination;
            set
            {
                this.command_ChangeBlockForNomination = value;
                this.OnPropertyChanged("Command_ChangeBlockForNomination");
            }
        }

        private RelayCommand<DanceNode> command_SetScore;
        public RelayCommand<DanceNode> Command_SetScore
        {
            get => this.command_SetScore;
            set
            {
                this.command_SetScore = value;
                this.OnPropertyChanged("Command_SetScore");
            }
        }

        private async Task UpdateMemberNum(int member_id, bool is_group, int value)
        {
            int check = await DanceRegDatabase.ExecuteNonQueryAsync("update nums_for_members set Number=" + value + " where Id_event=" + this.IdEvent + " and Id_member=" + member_id + " and Is_group=" + is_group);
            if (check == 0) await DanceRegDatabase.ExecuteNonQueryAsync("insert into nums_for_members values (" + this.IdEvent + ", " + member_id + ", " + is_group + ", " + value + ")");
        }
    }
}
