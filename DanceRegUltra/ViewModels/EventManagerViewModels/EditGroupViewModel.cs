using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Utilites;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class EditGroupViewModel : ViewModel
    {
        public bool IsEnableDancerEdit
        {
            get => this.DancerInWork?.MemberId > 0 ? false : true;
        }

        public string GroupType
        {
            get => MemberGroup.GetTypeByCount(this.Members.Count);
        }

        private int Id_event;
        private int Id_member;
        private MemberDancer DancerInWork;
        private static Regex Check_num = new Regex(@"^\d+$");

        private string memberNum;
        public string MemberNum
        {
            get => this.memberNum;
            set
            {
                if (Check_num.IsMatch(value))
                {
                    this.memberNum = value;
                }
                this.OnPropertyChanged("MemberNum");
            }
        }

        private LazyString firstname;
        public string Firstname
        {
            get => this.firstname.Value;
            set
            {
                this.firstname.Value = value;
                this.FindList.Find(this.Firstname, this.Surname);
                this.OnPropertyChanged("Firstname");
            }
        }

        private LazyString surname;
        public string Surname
        {
            get => this.surname.Value;
            set
            {
                this.surname.Value = value;
                this.FindList.Find(this.Firstname, this.Surname);
                this.OnPropertyChanged("Surname");
            }
        }

        public ListExt<IdTitle> Schools { get => DanceRegCollections.Schools.Value; }

        private IdTitle select_school;
        public IdTitle Select_school
        {
            get => this.select_school;
            set
            {
                this.select_school = value;
                //if (this.DancerInWork.MemberId == -1) this.DancerInWork.SetSchool(value);
                this.OnPropertyChanged("Select_school");
            }
        }

        public ListExt<MemberDancer> Members { get; private set; }

        private MemberDancer select_member;
        public MemberDancer Select_member
        {
            get => this.select_member;
            set
            {
                this.select_member = value;
                this.OnPropertyChanged("Select_member");
            }
        }

        public FindDancer FindList { get; private set; }

        public EditGroupViewModel(MemberGroup group)
        {
            string event_title = "";

            if (group.EventId > 0) event_title = "[" + DanceRegCollections.GetEventById(group.EventId).Title + "] ";

            this.Title = event_title + "Редактор групп - " + App.AppTitle;

            this.Id_event = group.EventId;
            this.Id_member = group.MemberId;
            this.memberNum = group.MemberNum.ToString();

            this.firstname = new LazyString("Firstname", 500, 0, this.Capitalize);
            this.firstname.SetValue("");
            this.firstname.Event_OnUpdateValue += this.Update;

            this.surname = new LazyString("Surname", 500, 0, this.Capitalize);
            this.surname.SetValue("");
            this.surname.Event_OnUpdateValue += this.Update;

            this.Select_school = group.School;
            this.Members = new ListExt<MemberDancer>(group.GroupMembers);

            this.FindList = new FindDancer(-1, 500);
            this.FindList.Event_FinishSearch += FindList_Event_FinishSearch;
            this.FindList.Event_changeSelectDancer += FindList_Event_changeSelectDancer;

            this.DancerInWork = new MemberDancer(-1, -1, "", "");
        }

        private void FindList_Event_changeSelectDancer(MemberDancer obj)
        {
            this.DancerInWork = obj;
            this.firstname.SetValue(this.DancerInWork.Name);
            this.surname.SetValue(this.DancerInWork.Surname);
            this.OnPropertyChanged("Firstname");
            this.OnPropertyChanged("Surname");
            this.OnPropertyChanged("IsEnableDancerEdit");
        }

        private void FindList_Event_FinishSearch()
        {
            this.OnPropertyChanged("FindList");
        }

        private string Capitalize(string value)
        {
            return App.CapitalizeAllWords(value);
        }

        private void Update(string value)
        {
            this.OnPropertyChanged(value);
            
            if (value == "Firstname") this.DancerInWork.SetName(this.Firstname);
            else if (value == "Surname") this.DancerInWork.SetSurname(this.Surname);
        }

        private async void AddMemberInGroupMethod()
        {
            MemberDancer tmp_dancer = DanceRegCollections.GetGroupDancerById(this.DancerInWork.MemberId);

            if (tmp_dancer == null)
            {
                if (this.DancerInWork.MemberId == -1)
                {
                    await DanceRegDatabase.ExecuteNonQueryAsync("insert into dancers (Firstname, Surname, Id_school) values ('" + this.Firstname + "', '" + this.Surname + "', " + this.Select_school.Id + ")");
                    DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select dancers.Id_member, dancers.Firstname, dancers.Surname, dancers.Id_school, schools.Name from dancers join schools using (Id_school) order by dancers.Id_member");
                    DbRow row = res[res.RowsCount - 1];
                    tmp_dancer = new MemberDancer(-1, row["Id_member"].ToInt32(), row["Firstname"].ToString(), row["Surname"].ToString());
                    tmp_dancer.SetSchool(DanceRegCollections.GetSchoolById(row["Id_school"].ToInt32()));
                }
                else tmp_dancer = this.DancerInWork;
                DanceRegCollections.AddGroupDancer(tmp_dancer);
            }

            this.Members.Add(tmp_dancer);
            this.Command_ClearMemberInGroup.Execute();

            this.OnPropertyChanged("GroupType");
        }

        private async void SaveGroupMethod()
        {
            List<int> id_members = new List<int>();
            foreach (DanceEvent event_edit in DanceRegCollections.Events)
            {
                MemberGroup group_edit = event_edit.GetGroupById(this.Id_member);
                if (group_edit != null)
                {
                    foreach(MemberDancer dancer in this.Members)
                    {
                        if (!id_members.Contains(dancer.MemberId)) id_members.Add(dancer.MemberId);
                        group_edit.AddMember(dancer);
                    }
                    
                    group_edit.SetSchool(this.Select_school);
                    if (group_edit.EventId == this.Id_event) group_edit.MemberNum = Convert.ToInt32(this.MemberNum);
                }
            }

            


            await DanceRegDatabase.ExecuteNonQueryAsync("update groups set Json_members='" + JsonConvert.SerializeObject(id_members) + "', Id_school=" + this.Select_school.Id + " where Id_member=" + this.Id_member);
            if (this.Id_event > 0) await DanceRegDatabase.ExecuteNonQueryAsync("update nums_for_members set Number=" + this.MemberNum + " where Id_event=" + this.Id_event + " and Id_member=" + this.Id_member + " and Is_group=1");

            base.Command_save?.Execute();
        }

        public RelayCommand Command_AddMemberInGroup
        {
            get => new RelayCommand(obj =>
            {
                this.AddMemberInGroupMethod();
            },
                (obj) => this.Firstname.Length > 0 && this.Surname.Length > 0);
        }

        public RelayCommand Command_ClearMemberInGroup
        {
            get => new RelayCommand(obj =>
            {
                this.FindList.Clear();
                this.FindList_Event_changeSelectDancer(new MemberDancer(-1, -1, "", ""));
            });
        }

        public RelayCommand<MemberDancer> Command_DeleteMemberInGroup
        {
            get => new RelayCommand<MemberDancer>(dancer =>
            {
                this.Members.Remove(dancer);
                this.OnPropertyChanged("GroupType");
            },
                (dancer) => dancer != null);
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveGroupMethod();
            },
                (obj) => this.Members != null && this.Members.Count >= 2);
        }
    }
}
