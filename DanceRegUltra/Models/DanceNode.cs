using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DanceRegUltra.Models
{
    public class DanceNode : INotifyPropertyChanged
    {
        public int EventId { get; private set; }
        public int NodeId { get; private set; }
        public IMember Member { get; private set; }
        public bool IsGroup { get; private set; }

        public IdTitle Platform { get; private set; }
        public IdTitle Block { get; private set; }

        public int LeagueId { get; private set; }
        public int AgeId { get; private set; }
        public int StyleId { get; private set; }

        private Lazy<List<int>> HideScores;
        public int[] Scores { get => this.HideScores.Value.ToArray(); }

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

        public DanceNode(int eventId, int nodeId, IMember member, bool isGroup, IdTitle platform, int leagueId, IdTitle block, int ageId, int styleId, int position = -1)
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

        public void SetScores(string jsonScores)
        {
            this.HideScores = new Lazy<List<int>>();
            this.HideScores.Value.AddRange(JsonConvert.DeserializeObject<List<int>>(jsonScores));
            this.OnPropertyChanged("Scores");
            this.OnPropertyChanged("JudgeCount");
        }

        public string GetScores()
        {
            return JsonConvert.SerializeObject(this.HideScores.Value);
        }

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
        //---метод OnPropertyChanged
    }
}
