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

namespace DanceRegUltra.Models
{
    public delegate void UpdateDanceEvent(int event_id, string column_name, object value_update);

    public class DanceEvent : INotifyPropertyChanged, IComparable<DanceEvent>
    {
        //public static DanceEvent Empty { get; }

        public int NodeId { get; private set; }

        public int JudgeCount { get; private set; }

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

        public DanceScheme SchemeEvent { get; private set; }

        public string JsonSchemeEvent { get; private set; }

        public DanceEvent(int id, string title, double startTimestamp, double endTimestamp, string json = "")
        {
            this.NodeId = -1;
            this.JudgeCount = 4;
            
            this.HideDancers = new Lazy<ListExt<MemberDancer>>();
            this.HideGroups = new Lazy<ListExt<MemberGroup>>();
            this.HideNodes = new Lazy<ListExt<DanceNode>>();
            
            this.IdEvent = id;
            this.title = title;
            this.startEventTimestamp = startTimestamp;
            this.EndEventTimestamp = endTimestamp;
            this.JsonSchemeEvent = json;
            
            this.event_updateDanceEvent = null;
            this.PropertyChanged = null;

            this.Command_EditEvent = MainViewModel.Command_EditEvent;
            this.Command_DeleteEvent = MainViewModel.Command_DeleteEvent;
        }
        
        public void AddNode(int node_id, int member_id, bool isGroup, int platform_id, int league_id, int block_id, int age_id, int style_id, string scores)
        {
            DanceNode newNode = new DanceNode(this.IdEvent, node_id, member_id, isGroup, platform_id, league_id, block_id, age_id, style_id);
            newNode.SetScores(scores);
            this.HideNodes.Value.Add(newNode);
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
        
        public void SetTitle(string newTitle)
        {
            this.Title = newTitle;
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "Title", newTitle);
        }

        public void SetStartTimestampEvent(double newTimestamp)
        {
            this.StartEventTimestamp = newTimestamp;
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "StartEventTimestamp", newTimestamp);
        }

        public void SetJsonScheme(string newJson)
        {
            this.JsonSchemeEvent = newJson;
            //this.SchemeEvent = DanceScheme.Deserialize(newJson);
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "JsonSchemeEvent", newJson);
        }

        public void SetEndTimestampEvent()
        {
            this.EndEventTimestamp = UnixTime.CurrentUnixTimestamp();
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "EndEventTimestamp", this.EndEventTimestamp);
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
