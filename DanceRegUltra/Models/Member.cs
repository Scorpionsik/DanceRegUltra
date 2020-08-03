using CoreWPF.MVVM;
using DanceRegUltra.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DanceRegUltra.Models
{
    public abstract class Member : NotifyPropertyChanged, IMember
    {
        private event UpdateMember event_UpdateMember;
        public event UpdateMember Event_UpdateMember
        {
            add
            {
                this.event_UpdateMember -= value;
                this.event_UpdateMember += value;
            }
            remove => this.event_UpdateMember -= value;
        }

        public int MemberId { get; private set; }
        public int EventId { get; private set; }

        private int memberNum;
        public int MemberNum
        {
            get => this.memberNum;
            set
            {
                this.memberNum = value;
                this.OnPropertyChanged("MemberNum");
                this.InvokeUpdate("MemberNum");
            }
        }

        private List<int> HideScores;

        public int[] Scores { get => this.HideScores.ToArray(); }

        public Member(int eventId, int memberId) : base()
        {
            this.EventId = eventId;
            this.MemberId = memberId;
            this.HideScores = new List<int>();
        }

        public string GetScores()
        {
            return JsonConvert.SerializeObject(this.HideScores);
        }

        public void SetScores(string jsonScores)
        {
            this.HideScores = JsonConvert.DeserializeObject<List<int>>(jsonScores);
            this.OnPropertyChanged("Scores");
            this.InvokeUpdate("Scores");
        }

        protected void InvokeUpdate(string dataColumn, object currentData = null, UpdateStatus status = UpdateStatus.Default, object replaceData = null)
        {
            this.event_UpdateMember?.Invoke(this.EventId, this.MemberId, dataColumn);
        }
    }
}
