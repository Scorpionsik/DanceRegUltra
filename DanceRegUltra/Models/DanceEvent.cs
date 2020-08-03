using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using System;
using System.Linq;

namespace DanceRegUltra.Models
{
    public struct DanceEvent
    {
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
        public IMember[] Members { get => this.HideMembers.Value.ToArray(); }

        public int IdEvent { get; private set; }
        public string Title { get; private set; }
        public double StartEventTimestamp { get; private set; }
        public double EndEventTimestamp { get; private set; }
        public string JsonSchemeEvent { get; private set; }

        public DanceEvent(int id, string title, int startTimestamp, string jsonScheme)
        {
            this.HideMembers = new Lazy<ListExt<IMember>>();
            this.IdEvent = id;
            this.Title = title;
            this.StartEventTimestamp = startTimestamp;
            this.EndEventTimestamp = -1;
            this.JsonSchemeEvent = jsonScheme;
            this.event_updateDanceEvent = null;
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
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "JsonSchemeEvent");
        }

        public void SetEndTimestampEvent()
        {
            this.EndEventTimestamp = UnixTime.CurrentUnixTimestamp();
            this.event_updateDanceEvent?.Invoke(this.IdEvent, "EndEventTimestamp");
        }
    }
}
