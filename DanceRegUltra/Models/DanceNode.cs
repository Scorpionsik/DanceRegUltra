using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DanceRegUltra.Models
{
    public delegate void UpdateDanceNode(int event_id, int node_id, string column_name, object value);

    public class DanceNode : NotifyPropertyChanged
    {
        public int EventId { get; private set; }
        public int NodeId { get; private set; }
        public Member Member { get; private set; }
        public bool IsGroup { get; private set; }

        public NodeStatus Status
        {
            get
            {
                NodeStatus result = NodeStatus.Default;
                if (this.PrizePlace > 0) result = this.IsPrintPrize ? NodeStatus.Print : NodeStatus.GetScores;
                return result;
            }
        }

        private IdTitle platform;
        public IdTitle Platform
        {
            get => this.platform;
            private set
            {
                this.platform = value;
                this.event_UpdateDanceNode?.Invoke(this.EventId, this.NodeId, "Id_platform", value.Id);
                this.OnPropertyChanged("Platform");
            }
        }
        public IdTitle Block { get; private set; }

        public int LeagueId { get; private set; }
        public int AgeId { get; private set; }
        public int StyleId { get; private set; }

        private Lazy<List<int>> HideScores;
        public int[] Scores { get => this.HideScores.Value.ToArray(); }

        private int prizePlace;
        public int PrizePlace
        {
            get => this.prizePlace;
            private set
            {
                this.prizePlace = value;
                this.event_UpdateDanceNode?.Invoke(this.EventId, this.NodeId, "Prize_place", value);
                this.OnPropertyChanged("PrizePlace");
            }
        }

        private event UpdateDanceNode event_UpdateDanceNode;
        public event UpdateDanceNode Event_UpdateDanceNode
        {
            add
            {
                this.event_UpdateDanceNode -= value;
                this.event_UpdateDanceNode += value;
            }
            remove => this.event_UpdateDanceNode -= value;
        }

        private bool isPrintPrize;
        public bool IsPrintPrize
        {
            get => this.isPrintPrize;
            private set
            {
                this.isPrintPrize = value;
                this.event_UpdateDanceNode?.Invoke(this.EventId, this.NodeId, "Is_print_prize", value);
                this.OnPropertyChanged("IsPrintPrize");
            }
        }

        public int JudgeCount
        {
            get => this.HideScores.Value.Count;
        }

        private int position;
        public int Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.OnPropertyChanged("Position");
            }
        }

        public DanceNode(int eventId, int nodeId, Member member, bool isGroup, IdTitle platform, int leagueId, IdTitle block, int ageId, int styleId, int position = -1)
        {
            this.EventId = eventId;
            this.NodeId = nodeId;

            this.Member = member;
            this.IsGroup = isGroup;

            this.Platform = platform;
            this.LeagueId = leagueId;
            this.Block = block;
            this.AgeId = ageId;
            this.StyleId = styleId;

            this.HideScores = new Lazy<List<int>>();

            this.position = position;
        }

        public void SetPrizePlace(int place)
        {
            if (place != this.PrizePlace)
            {
                this.IsPrintPrize = false;
                this.PrizePlace = place;
                this.OnPropertyChanged("Status");
            }
        }

        public void Print()
        {
            this.IsPrintPrize = true;
            this.OnPropertyChanged("Status");
        }

        public void SetScores(string jsonScores)
        {
            this.HideScores = new Lazy<List<int>>();
            if(jsonScores != null && jsonScores.Length > 0) this.HideScores.Value.AddRange(JsonConvert.DeserializeObject<List<int>>(jsonScores));
            this.OnPropertyChanged("Scores");
            this.OnPropertyChanged("JudgeCount");
        }

        public void SetPlatform(IdTitle platform)
        {
            this.Platform = platform;
        }

        public string GetScores()
        {
            return JsonConvert.SerializeObject(this.HideScores.Value);
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
    }
}
