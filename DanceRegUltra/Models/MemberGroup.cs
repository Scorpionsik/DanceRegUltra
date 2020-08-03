using CoreWPF.Utilites;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DanceRegUltra.Models
{
    public class MemberGroup : Member
    {
        private ListExt<int> HideGroupMembersId;
        public int[] GroupMembersId { get => this.HideGroupMembersId.ToArray(); }

        public MemberGroup(int eventId, int memberId, IEnumerable<int> group) : base(eventId, memberId)
        {
            this.HideGroupMembersId = new ListExt<int>(group);
        }

        public MemberGroup(int eventId, int memberId, string group) : base(eventId, memberId)
        {
            this.HideGroupMembersId = new ListExt<int>(JsonConvert.DeserializeObject<ListExt<int>>(group));
        }
        
        public void AddGroupMember(int memberId)
        {
            if (!this.HideGroupMembersId.Contains(memberId))
            {
                this.HideGroupMembersId.Add(memberId);
                this.InvokeUpdate("MembersId", memberId, Interfaces.UpdateStatus.Add);
            }
        }

        public void RemoveGroupMember(int memberId)
        {
            if (this.HideGroupMembersId.Contains(memberId))
            {
                this.HideGroupMembersId.Remove(memberId);
                this.InvokeUpdate("MembersId", memberId, Interfaces.UpdateStatus.Delete);
            }
        }
    }
}
