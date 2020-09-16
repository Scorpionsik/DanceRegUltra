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

    public class DanceEvent : INotifyPropertyChanged, IComparable<DanceEvent>
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

        private event Action event_addNodes;
        public event Action Event_addNodes
        {
            add
            {
                this.event_addNodes -= value;
                this.event_addNodes += value;
            }
            remove => this.event_addNodes -= value;
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

                await DanceRegDatabase.ExecuteNonQueryAsync("insert into nominations values (" + this.IdEvent + ", " + node.Block.Id + ", " + node.LeagueId + ", " + node.AgeId + ", " + node.StyleId +", "+ !res.HasRows +",'"+ JsonConvert.SerializeObject(new List<bool>() { false, false, false, false }) +"')");
                
                DanceNomination nomination = new DanceNomination(this.IdEvent, node.Block, this.SchemeEvent.GetSchemeArrayById(node.Block.Id, Enums.SchemeType.Block).ScoreType, node.LeagueId, node.AgeId, node.StyleId, !res.HasRows);
                this.AddNomination(nomination);
                tmp_nomination = nomination;
            }
            tmp_nomination.AddNominant(node);
            this.event_addNodes?.Invoke();
        }

        public void AddNomination(DanceNomination nomination)
        {
            nomination.Event_UpdateNominant += this.UpdateNominantPosition;
            int index = 0;
            while (index < this.HideNominations.Value.Count && this.SchemeEvent.Compare(this.HideNominations.Value[index], nomination) != 1) index++;
            this.HideNominations.Value.Insert(index, nomination);
            this.OnPropertyChanged("Nominations");
        }

        public void DeleteNomination(int block_id, int league_id, int age_id, int style_id)
        {
            int index = 0;
            while (index < this.HideNominations.Value.Count &&
                this.HideNominations.Value[index].Block_info.Id != block_id &&
                this.HideNominations.Value[index].League_id != league_id &&
                this.HideNominations.Value[index].Age_id != age_id &&
                this.HideNominations.Value[index].Style_id != style_id) index++;
            if (index < this.HideNominations.Value.Count)
            {
                this.HideNominations.Value[index].Event_UpdateNominant -= this.UpdateNominantPosition;
                this.HideNominations.Value.RemoveAt(index);
            }
        }

        private async void UpdateNominantPosition(DanceNode node)
        {
            int index = this.HideNodes.Value.IndexOf(node);
            await this.UpdateNodePosition(index, this.HideNodes.Value.Count - 1);
        }

        private async void UpdateDanceNode(int event_id, int node_id, string column_name, object value)
        {
            await DanceRegDatabase.ExecuteNonQueryAsync("update event_nodes set " + column_name + "='" + value + "' where Id_event=" + event_id + " and Id_node=" + node_id);
        }

        public void AddNode(int node_id, Member member, bool isGroup, IdTitle platform, int league_id, IdTitle block, int age_id, int style_id, string scores)
        {
            DanceNode newNode = new DanceNode(this.IdEvent, node_id, member, isGroup, platform, league_id, block, age_id, style_id);
            newNode.Event_UpdateDanceNode += this.UpdateDanceNode;
            newNode.SetScores(scores);
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

        public async Task AddNodeAsync(Member member, bool isGroup, IdTitle platform, int league_id, IdTitle block, int age_id, int style_id)
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
                this.AddNominationMember(newNode);
                this.AddSchool(newNode.Member.School);
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into event_nodes values (" + this.IdEvent + ", " + newNode.NodeId + ", " + newNode.Member.MemberId + ", " + isGroup + ", " + newNode.Platform.Id + ", " + newNode.LeagueId + ", " + newNode.Block.Id + ", " + newNode.AgeId + ", " + newNode.StyleId + ", '', 0, 0, " + position + ")");
                if (position < this.HideNodes.Value.Count - 1) await this.UpdateNodePosition(position, this.HideNodes.Value.Count - 1);
                //this.HideNodes.Value.Add(newNode);
            }
        }

        public async Task DeleteNodeAsync(DanceNode node)
        {
            int position = this.HideNodes.Value.IndexOf(node);
            if (position > -1)
            {
                //if (node.IsPrintPrice) this.Member_finish_count--;
                node.Event_UpdateDanceNode -= this.UpdateDanceNode;
                this.HideNodes.Value.RemoveAt(position);
                await DanceRegDatabase.ExecuteNonQueryAsync("delete from event_nodes where Id_event=" + this.IdEvent + " and Id_node=" + node.NodeId);
                if (position < this.HideNodes.Value.Count - 1) await this.UpdateNodePosition(position, this.HideNodes.Value.Count - 1);
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from event_nodes where Id_event=" + this.IdEvent + " and Id_member=" + node.Member.MemberId);
                if (res.RowsCount == 0)
                {
                    if (node.IsGroup)
                    {
                        this.HideGroups.Value.Remove((MemberGroup)node.Member);
                    }
                    else this.HideDancers.Value.Remove((MemberDancer)node.Member);
                    
                    //this.All_members_count--;
                }

                res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from nominations where Id_event=" + this.IdEvent + " and Id_block=" + node.Block.Id + " and Id_league=" + node.LeagueId + " and Id_age=" + node.AgeId + " and Id_style=" + node.StyleId);
                if (!res.HasRows)
                {
                    await DanceRegDatabase.ExecuteNonQueryAsync("delete from nominations where Id_event=" + this.IdEvent + " and Id_block=" + node.Block.Id + " and Id_league=" + node.LeagueId + " and Id_age=" + node.AgeId + " and Id_style=" + node.StyleId);
                    this.DeleteNomination(node.Block.Id, node.LeagueId, node.AgeId, node.StyleId);
                }

                this.RemoveSchool(node.Member.School);
            }
        }

        public async Task UpdateNodePosition(int index1, int index2)
        {
            int[] minmax = new int[2] { Math.Min(index1, index2), Math.Max(index1, index2) };

            for (int i = minmax[0]; i <= minmax[1]; i++)
            {
                //DanceRegCollections.Ages.Value[i].Position = i + 1;
                await DanceRegDatabase.ExecuteNonQueryAsync("update event_nodes set Position=" + i + " where Id_event=" + this.IdEvent + " and Id_node=" + this.Nodes[i].NodeId);
            }
        }

        public void AddMember(Member newMember)
        {
            int index_sort = 0;
            if(newMember is MemberDancer dancer)
            {
                MemberDancer tmp_add = null;
                if (dancer.EventId != this.IdEvent)
                {
                    tmp_add = new MemberDancer(this.IdEvent, dancer.MemberId, dancer.Name, dancer.Surname);
                    tmp_add.SetSchool(dancer.School);
                }
                else tmp_add = dancer;
                while (index_sort < this.HideDancers.Value.Count && this.HideDancers.Value[index_sort].CompareTo(tmp_add) <= 0) index_sort++;
                this.HideDancers.Value.Insert(index_sort, tmp_add);
                this.OnPropertyChanged("Dancers");
            }
            else if(newMember is MemberGroup group)
            {
                MemberGroup tmp_add = null;
                if (group.EventId != this.IdEvent)
                {
                    tmp_add = new MemberGroup(this.IdEvent, group.MemberId, group.GroupMembers);
                }
                else tmp_add = group;
                while (index_sort < this.HideGroups.Value.Count && this.HideGroups.Value[index_sort].CompareTo(tmp_add) <= 0) index_sort++;
                this.HideGroups.Value.Insert(index_sort, tmp_add);
                this.OnPropertyChanged("Groups");
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

        /// <summary>
        /// Событие для обновления привязанного объекта (в XAML)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для обновления выбранного привязанного объекта (в XAML)
        /// </summary>
        /// <param name="prop">Принимает строку-имя объекта, который необходимо обновить</param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public int CompareTo(DanceEvent obj)
        {
            double res = this.StartEventTimestamp - obj.StartEventTimestamp;

            if (res == 0) return 0;
            else return res < 0 ? -1 : 1;
        }
        //---метод OnPropertyChanged
    }
}
