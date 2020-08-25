using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DanceRegUltra.Models
{
    public struct DanceNode : INotifyPropertyChanged
    {
        public int EventId { get; private set; }
        public int NodeId { get; private set; }
        public int MemberId { get; private set; }
        public bool IsGroup { get; private set; }

        public int PlatformId { get; private set; }
        public int BlockId { get; private set; }

        public int LeagueId { get; private set; }
        public int AgeId { get; private set; }
        public int StyleId { get; private set; }

        private Lazy<List<int>> HideScores;
        public int[] Scores { get => this.HideScores.Value.ToArray(); }


        public DanceNode(int eventId, int nodeId, int memberId, bool isGroup, int platformId, int leagueId, int blockId, int ageId, int styleId)
        {
            this.EventId = eventId;
            this.NodeId = nodeId;

            this.MemberId = memberId;
            this.IsGroup = isGroup;

            this.PlatformId = platformId;
            this.LeagueId = leagueId;
            this.BlockId = blockId;
            this.AgeId = ageId;
            this.StyleId = styleId;

            this.HideScores = new Lazy<List<int>>();
            this.PropertyChanged = null;
        }

        public void SetScores(string jsonScores)
        {
            this.HideScores = new Lazy<List<int>>();
            this.HideScores.Value.AddRange(JsonConvert.DeserializeObject<List<int>>(jsonScores));
            this.OnPropertyChanged("Scores");
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
