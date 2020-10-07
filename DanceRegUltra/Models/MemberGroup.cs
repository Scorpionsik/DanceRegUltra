using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;

namespace DanceRegUltra.Models
{
    public class MemberGroup : Member, IComparable<MemberGroup>
    {
        private Lazy<ListExt<MemberDancer>> HideGroupMembers;
        public ListExt<MemberDancer> GroupMembers { get => this.HideGroupMembers?.Value; }

        private string groupType;
        public string GroupType
        {
            get
            {
                this.groupType = MemberGroup.GetTypeByCount(this.GroupMembers.Count);
                return this.groupType;
            }
        }

        public static string GetTypeByCount(int count)
        {
            int countGroup = count;
            string groupType = "";

            if (countGroup < 2) groupType = "Группа";
            else if (countGroup >= 2 && countGroup <= 6) groupType = "Группа";
            else if (countGroup >= 7 && countGroup <= 23) groupType = "Формейшен";
            else if (countGroup >= 24) groupType = "Продакшн";

            return groupType;
        }

        public string GroupMembersString
        {
            get
            {
                string return_string = "";
                if (this.MemberId == -1) return_string += "Новая";
                else
                {
                    int count = 0;
                    foreach(MemberDancer dancer in this.GroupMembers)
                    {
                        return_string += dancer.Surname;
                        if (count < this.GroupMembers.Count - 1) return_string += ", ";
                        count++;
                    }
                }
                return return_string;
            }
        }

        private MemberGroup(int eventId, int memberId) : base(eventId, memberId)
        {
            this.HideGroupMembers = new Lazy<ListExt<MemberDancer>>();
            this.HideGroupMembers.Value.CollectionChanged += this.UpdateMembersMethod;
            this.groupType = "Группа";
        }

        ~MemberGroup()
        {
            this.HideGroupMembers.Value.CollectionChanged -= this.UpdateMembersMethod;
        }

        public void UpdateGroupMembersString()
        {
            this.OnPropertyChanged("GroupMembersString");
        }

        public MemberGroup(int eventId, int memberId, IEnumerable<MemberDancer> group) : this(eventId, memberId)
        {
            this.HideGroupMembers.Value.AddRange(group);
        }

        public MemberGroup(int eventId, int memberId, string group) : this(eventId, memberId)
        {
            this.Initialize(group);
        }
        
        private async void Initialize(string group)
        {
            ListExt<int> group_id = JsonConvert.DeserializeObject<ListExt<int>>(group);
            string query = "select * from dancers where ";
            for(int i = 0; i < group_id.Count; i++)
            {
                query += "Id_member=" + group_id[i];
                if (i != group_id.Count - 1) query += " or ";
            }
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync(query);
            MemberDancer tmp_dancer = null;
            foreach (DbRow row in res)
            {
                tmp_dancer = DanceRegCollections.GetGroupDancerById(row.GetInt32("Id_member"));
                if (tmp_dancer == null)
                {
                    tmp_dancer = new MemberDancer(-1, row.GetInt32("Id_member"), row["Firstname"].ToString(), row["Surname"].ToString());
                    DanceRegCollections.AddGroupDancer(tmp_dancer);
                }
                this.HideGroupMembers.Value.Add(tmp_dancer);
            }
        }

        private void UpdateMembersMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("GroupMembers");
            this.OnPropertyChanged("GroupType");
            this.OnPropertyChanged("GroupMembersString");
        }

        public void AddMember(int id_member, string name, string surname)
        {
            foreach (MemberDancer d in this.HideGroupMembers.Value)
            {
                if (d.MemberId == id_member)
                {
                    return;
                }
            }

            MemberDancer tmp_dancer = DanceRegCollections.GetGroupDancerById(id_member);
            if (tmp_dancer == null)
            {
                tmp_dancer = new MemberDancer(-1, id_member, name, surname);
                DanceRegCollections.AddGroupDancer(tmp_dancer);
            }
            this.HideGroupMembers.Value.Add(tmp_dancer);
        }

        public void AddMember(MemberDancer dancer)
        {
            foreach (MemberDancer d in this.HideGroupMembers.Value)
            {
                if (d.MemberId == dancer.MemberId)
                {
                    return;
                }
            }

            this.HideGroupMembers.Value.Add(dancer);
        }

        public void RemoveMember(MemberDancer dancer)
        {
            foreach(MemberDancer d in this.HideGroupMembers.Value)
            {
                if(d.MemberId == dancer.MemberId)
                {
                    this.HideGroupMembers.Value.Remove(d);
                    break;
                }
            }
        }

        public string GetMembers()
        {
            ListExt<int> result = new ListExt<int>();
            foreach(MemberDancer dancer in this.HideGroupMembers.Value)
            {
                result.Add(dancer.MemberId);
            }
            return JsonConvert.SerializeObject(result);
        }

        public int CompareTo(MemberGroup other)
        {
            return this.GroupType.CompareTo(other.GroupType);
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

        private RelayCommand<MemberGroup> command_AddGroupUseMember;
        public RelayCommand<MemberGroup> Command_AddGroupUseMember
        {
            get => this.command_AddGroupUseMember;
            set
            {
                this.command_AddGroupUseMember = value;
                this.OnPropertyChanged("Command_AddGroupUseMember");
            }
        }

        private RelayCommand<MemberGroup> command_EditGroup;
        public RelayCommand<MemberGroup> Command_EditGroup
        {
            get => this.command_EditGroup;
            set
            {
                this.command_EditGroup = value;
                this.OnPropertyChanged("Command_EditGroup");
            }
        }
    }
}
