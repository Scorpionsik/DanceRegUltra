using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Static;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DanceRegUltra.Models
{
    public class MemberGroup : Member
    {
        private ListExt<MemberDancer> HideGroupMembers;
        public MemberDancer[] GroupMembers { get => this.HideGroupMembers.ToArray(); }

        public MemberGroup(int eventId, int memberId, IEnumerable<MemberDancer> group) : base(eventId, memberId)
        {
            this.HideGroupMembers = new ListExt<MemberDancer>(group);
        }

        public MemberGroup(int eventId, int memberId, string group) : base(eventId, memberId)
        {
            this.HideGroupMembers = new ListExt<MemberDancer>();
            this.Initialize(group);
        }
        
        private async void Initialize(string group)
        {
            List<int> group_id = JsonConvert.DeserializeObject<List<int>>(group);
            string query = "select * from dancers where ";
            for(int i = 0; i < group_id.Count; i++)
            {
                query += "Id_dancer=" + group_id[i];
                if (i != group_id.Count - 1) query += " or ";
            }
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync(query);

            foreach(DbRow row in res)
            {
                this.HideGroupMembers.Add(new MemberDancer(this.EventId, row["Id_dancer"].ToInt32(), row["Firstname"].ToString(), row["Surname"].ToString()));
            }
        }

        public void AddMember(int id_member, string name, string surname)
        {
            foreach (MemberDancer d in this.HideGroupMembers)
            {
                if (d.MemberId == id_member)
                {
                    return;
                }
            }
            this.HideGroupMembers.Add(new MemberDancer(this.EventId, id_member, name, surname));
        }

        public void RemoveMember(MemberDancer dancer)
        {
            foreach(MemberDancer d in this.HideGroupMembers)
            {
                if(d.MemberId == dancer.MemberId)
                {
                    this.HideGroupMembers.Remove(d);
                    break;
                }
            }
        }
        /*
        public void AddGroupMember(int memberId)
        {
            if (!this.HideGroupMembers.Contains(memberId))
            {
                this.HideGroupMembers.Add(memberId);
                this.InvokeUpdate("MembersId", memberId, UpdateStatus.Add);
            }
        }

        public void RemoveGroupMember(int memberId)
        {
            if (this.HideGroupMembers.Contains(memberId))
            {
                this.HideGroupMembers.Remove(memberId);
                this.InvokeUpdate("MembersId", memberId, UpdateStatus.Delete);
            }
        }
        */
    }
}
