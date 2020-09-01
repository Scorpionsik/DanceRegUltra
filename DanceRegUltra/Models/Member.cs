using CoreWPF.MVVM;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DanceRegUltra.Models
{
    public delegate void UpdateMember(int eventId, int memberId, string dataColumn, object currentData = null, UpdateStatus status = UpdateStatus.Default, object replaceData = null);

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

        public IdTitle School { get; private set; }

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

        public Member(int eventId, int memberId) : base()
        {
            this.EventId = eventId;
            this.MemberId = memberId;
            this.MemberNum = 0;
        }

        public void SetSchool(IdTitle school)
        {
            this.School = school;
            this.OnPropertyChanged("SetSchool");
        }

        protected void InvokeUpdate(string dataColumn, object currentData = null, UpdateStatus status = UpdateStatus.Default, object replaceData = null)
        {
            this.event_UpdateMember?.Invoke(this.EventId, this.MemberId, dataColumn);
        }
    }
}
