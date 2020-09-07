﻿using CoreWPF.Utilites;
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

        public string GroupType
        {
            get
            {
                int countGroup = this.HideGroupMembers.Value.Count;
                string type = "";

                if (countGroup < 2) type = "ошибка";
                else if(countGroup >= 2 && countGroup <= 6) type = "группа";
                else if (countGroup >= 7 && countGroup <= 23) type = "формейшен";
                else if (countGroup >= 24) type = "продакшн";

                return type;
            }
        }

        private MemberGroup(int eventId, int memberId) : base(eventId, memberId)
        {
            this.HideGroupMembers = new Lazy<ListExt<MemberDancer>>();
            this.HideGroupMembers.Value.CollectionChanged += this.UpdateMembersMethod;
        }

        ~MemberGroup()
        {
            this.HideGroupMembers.Value.CollectionChanged -= this.UpdateMembersMethod;
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
                query += "Id_dancer=" + group_id[i];
                if (i != group_id.Count - 1) query += " or ";
            }
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync(query);
            MemberDancer tmp_dancer = null;
            foreach (DbRow row in res)
            {
                tmp_dancer = DanceRegCollections.GetGroupDancerById(row["Id_dancer"].ToInt32());
                if (tmp_dancer == null)
                {
                    tmp_dancer = new MemberDancer(-1, row["Id_dancer"].ToInt32(), row["Firstname"].ToString(), row["Surname"].ToString());
                    DanceRegCollections.AddGroupDancer(tmp_dancer);
                }
                this.HideGroupMembers.Value.Add(tmp_dancer);
            }
        }

        private void UpdateMembersMethod(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("GroupMembers");
            this.OnPropertyChanged("GroupType");
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
            return JsonConvert.SerializeObject(this.HideGroupMembers);
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
    }
}
