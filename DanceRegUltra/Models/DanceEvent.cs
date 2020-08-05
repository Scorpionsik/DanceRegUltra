using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DanceRegUltra.Models
{
    public struct DanceEvent : INotifyPropertyChanged
    {
        public int NodeId { get; private set; }

        public int JudgeCount { get; private set; }

        private event Action<int, string> event_updateDanceEvent;
        /// <summary>
        /// Срабатывает при изменении значений события; передает в числовой переменной id данного события, в строке название столбца в таблице бд
        /// </summary>
        public event Action<int, string> Event_UpdateDanceEvent
        {
            add
            {
                this.event_updateDanceEvent -= value;
                this.event_updateDanceEvent += value;
            }
            remove => this.event_updateDanceEvent -= value;
        }

        private Lazy<ListExt<IMember>> HideMembers;
        public IMember[] Members { get => this.HideMembers?.Value?.ToArray(); }

        private Lazy<ListExt<DanceNode>> HideNodes;
        public DanceNode[] Nodes { get => this.HideNodes?.Value?.ToArray(); }

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

        public DanceEvent(int id, string title, int startTimestamp, string jsonScheme, int nodeId, int judgeCount)
        {
            this.NodeId = nodeId;
            this.JudgeCount = judgeCount;
            this.HideMembers = new Lazy<ListExt<IMember>>();
            this.HideNodes = new Lazy<ListExt<DanceNode>>();
            this.IdEvent = id;
            this.title = title;
            this.startEventTimestamp = startTimestamp;
            this.EndEventTimestamp = -1;
            this.JsonSchemeEvent = jsonScheme;
            this.SchemeEvent = DanceScheme.Deserialize(this.JsonSchemeEvent);

            this.event_updateDanceEvent = null;
            this.PropertyChanged = null;
        }

        public void SetTitle(string newTitle)
        {
            this.Title = newTitle;
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "Title");
        }

        public void SetStartTimestampEvent(double newTimestamp)
        {
            this.StartEventTimestamp = newTimestamp;
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "StartEventTimestamp");
        }

        public void SetJsonScheme(string newJson)
        {
            this.JsonSchemeEvent = newJson;
            this.SchemeEvent = DanceScheme.Deserialize(newJson);
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "JsonSchemeEvent");
        }

        public void SetEndTimestampEvent()
        {
            this.EndEventTimestamp = UnixTime.CurrentUnixTimestamp();
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "EndEventTimestamp");
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
