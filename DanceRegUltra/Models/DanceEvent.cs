using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.ViewModels;
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
        //public static DanceEvent Empty { get; }

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

        private int judgeCount;
        public int JudgeCount
        {
            get => this.judgeCount;
            private set
            {
                this.judgeCount = value;
                this.OnPropertyChanged("JudgeCount");
                this.event_updateDanceEvent?.Invoke(this.IdEvent, "Judge_count", value);
            }
        }

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


        private Lazy<ListExt<MemberDancer>> HideDancers;
        public ListExt<MemberDancer> Dancers { get => this.HideDancers?.Value; }

        private Lazy<ListExt<MemberGroup>> HideGroups;
        public ListExt<MemberGroup> Groups { get => this.HideGroups?.Value; }

        private Lazy<ListExt<DanceNode>> HideNodes;
        public ListExt<DanceNode> Nodes { get => this.HideNodes?.Value; }

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

        //public string JsonSchemeEvent { get; private set; }

        public DanceEvent(int id, string title, double startTimestamp, double endTimestamp, string json = "", int node_id = 1, int judge_count = 4)
        {
            this.NodeId = node_id;
            this.JudgeCount = judge_count;

            this.HideDancers = new Lazy<ListExt<MemberDancer>>();
            this.HideGroups = new Lazy<ListExt<MemberGroup>>();
            this.HideNodes = new Lazy<ListExt<DanceNode>>();

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

            this.Leagues = new Dictionary<int, List<IdTitle>>();
            this.Ages = new Dictionary<int, List<IdTitle>>();
            this.Styles = new List<int>();
        }

        public void AddNode(int node_id, IMember member, bool isGroup, IdTitle platform, int league_id, IdTitle block, int age_id, int style_id, string scores)
        {
            DanceNode newNode = new DanceNode(this.IdEvent, node_id, member, isGroup, platform, league_id, block, age_id, style_id);
            newNode.SetScores(scores);
            this.HideNodes.Value.Add(newNode);
        }

        public async Task AddNodeAsync(IMember member, bool isGroup, IdTitle platform, int league_id, IdTitle block, int age_id, int style_id)
        {
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from event_nodes where Id_event=" + this.IdEvent + " and Id_member=" + member.MemberId + " and Is_group=" + isGroup + " and Id_platform=" + platform.Id + " and Id_league=" + league_id + " and Id_block=" + block.Id + " and Id_age=" + age_id + " and Id_style=" + style_id);
            if (res.RowsCount == 0)
            {
                DanceNode newNode = new DanceNode(this.IdEvent, this.NodeId++, member, isGroup, platform, league_id, block, age_id, style_id);
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

                await DanceRegDatabase.ExecuteNonQueryAsync("insert into event_nodes values (" + this.IdEvent + ", " + newNode.NodeId + ", " + newNode.Member.MemberId + ", " + isGroup + ", " + newNode.Platform.Id + ", " + newNode.LeagueId + ", " + newNode.Block.Id + ", " + newNode.AgeId + ", " + newNode.StyleId + ", '', " + position + ")");
                if (position < this.HideNodes.Value.Count - 1) await this.UpdateNodePosition(position, this.HideNodes.Value.Count - 1);
                //this.HideNodes.Value.Add(newNode);
            }
        }

        public async Task DeleteNodeAsync(DanceNode node)
        {
            int position = this.HideNodes.Value.IndexOf(node);

            if(position > -1)
            {
                this.HideNodes.Value.RemoveAt(position);
                await DanceRegDatabase.ExecuteNonQueryAsync("delete from event_nodes where Id_event=" + this.IdEvent + " and Id_node=" + node.NodeId);
                if (position < this.HideNodes.Value.Count - 1) await this.UpdateNodePosition(position, this.HideNodes.Value.Count - 1);
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

        public void AddMember(IMember newMember)
        {
            if(newMember is MemberDancer dancer)
            {
                MemberDancer tmp_add = null;
                if (dancer.EventId != this.IdEvent)
                {
                    tmp_add = new MemberDancer(this.IdEvent, dancer.MemberId, dancer.Name, dancer.Surname);
                }
                else tmp_add = dancer;
                this.HideDancers.Value.Add(tmp_add);
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
                this.HideGroups.Value.Add(group);
                this.OnPropertyChanged("Groups");
            }
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
