using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private Lazy<List<List<double>>> HideScores;
        public List<List<double>> Scores { get => this.HideScores.Value; }

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

            this.HideScores = new Lazy<List<List<double>>>();

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
        
        public double GetAverage(IEnumerable<bool> ignore, JudgeType type, bool separate)
        {
            double result = 0;

            if (!separate) result = this.GetAverageAllMembers(ignore, type);
            else result = this.GetAverageSeparateDancersGroup(ignore, type);

            return result;
        }

        public double GetAverage(IEnumerable<IdCheck> ignore, JudgeType type, bool separate)
        {
            List<bool> tmp_send = new List<bool>();
            foreach(IdCheck check in ignore)
            {
                tmp_send.Add(check.IsChecked);
            }
            return this.GetAverage(tmp_send, type, separate);
        }

        private double GetAverageAllMembers(IEnumerable<bool> ignore, JudgeType type)
        {
            double result = 0;
            int step = 0, current = 0;

            foreach (bool judge_ignore in ignore)
            {
                if (!judge_ignore)
                {
                    current++;
                    double sum = 0;
                    if (step < this.Scores.Count)
                    {
                        int score_count = type == JudgeType.ThreeD ? 3 : 4;
                        for (int i = 0; i < score_count; i++)
                        {
                            sum += this.Scores[step][i];
                        }

                        result += Convert.ToDouble(sum / score_count);
                    }
                }
                step++;
            }

            return current > 0 ? Convert.ToDouble(result / current) : 0;
        }

        private double GetAverageSeparateDancersGroup(IEnumerable<bool> ignore, JudgeType type)
        {
            return 0;
        }

        public void Print()
        {
            this.IsPrintPrize = true;
            this.OnPropertyChanged("Status");
        }

        public void SetScores(string jsonScores)
        {
            this.HideScores = new Lazy<List<List<double>>>();
            if (jsonScores != null && jsonScores.Length > 0) this.HideScores.Value.AddRange(JsonConvert.DeserializeObject <List<List<double>>> (jsonScores));
            this.OnPropertyChanged("Scores");
            this.OnPropertyChanged("JudgeCount");
        }

        public void SetScores(IEnumerable<IEnumerable<double>> scores)
        {
            if(this.HideScores == null) this.HideScores = new Lazy<List<List<double>>>();
            //для обновления
            for(int i = 0; i < this.HideScores.Value.Count && i < scores.Count(); i++)
            {
                if (this.HideScores.Value[i].Count < scores.ElementAt(i).Count()) this.HideScores.Value[i].Add(0);
                for(int j = 0; j < this.HideScores.Value[i].Count && j < scores.ElementAt(i).Count(); j++)
                {
                    this.HideScores.Value[i][j] = scores.ElementAt(i).ElementAt(j);
                }
            }
            //для добавления
            for (int i = 0; i < scores.Count() - this.HideScores.Value.Count; i++)
            {
                this.HideScores.Value.Add(new List<double>(scores.ElementAt(this.HideScores.Value.Count + i)));
            }
            //для удаления
            for (int i = 0; i < this.HideScores.Value.Count - scores.Count(); i++)
            {
                this.HideScores.Value.RemoveAt(this.HideScores.Value.Count - 1 - i);
            }
                        
            foreach(List<double> new_score in scores)
            {
                this.HideScores.Value.Add(new_score);
            }
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
    }
}
