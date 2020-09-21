using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Utilites;
using DanceRegUltra.Views.EventManagerViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class EditDancerViewModel : ViewModel
    {


        private int Id_event;
        private int Id_member;
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
                this.OnPropertyChanged("Select_school");
            }
        }

        public EditDancerViewModel(MemberDancer dancer)
        {
            string event_title = "";

            if (dancer.EventId > 0) event_title = "["+ DanceRegCollections.GetEventById(dancer.EventId).Title +"] ";

            this.Title = event_title + "Редактор танцоров - " + App.AppTitle;

            this.firstname = new LazyString("Firstname", 500, 0, this.Capitalize);
            this.firstname.Event_OnUpdateValue += this.Update;
            this.firstname.SetValue(dancer.Name);

            this.surname = new LazyString("Surname", 500, 0, this.Capitalize);
            this.surname.Event_OnUpdateValue += this.Update;
            this.surname.SetValue(dancer.Surname);

            this.Id_event = dancer.EventId;
            this.Id_member = dancer.MemberId;
            this.MemberNum = this.Id_event > 0 ? dancer.MemberNum.ToString() : "";
            
            this.Select_school = dancer.School;
        }

        private string Capitalize(string value)
        {
            return App.CapitalizeAllWords(value);
        }

        private void Update(string value)
        {
            this.OnPropertyChanged(value);
        }

        private async void SaveDancerMethod()
        {
            MemberDancer dancer_group = DanceRegCollections.GetGroupDancerById(this.Id_member);
            if (dancer_group != null)
            {
                dancer_group.SetName(this.Firstname);
                dancer_group.SetSurname(this.Surname);
                dancer_group.SetSchool(this.Select_school);
            }

            foreach (DanceEvent event_edit in DanceRegCollections.Events)
            {
                MemberDancer dancer_edit = event_edit.GetDancerById(this.Id_member);
                if (dancer_edit != null)
                {
                    dancer_edit.SetName(this.Firstname);
                    dancer_edit.SetSurname(this.Surname);
                    dancer_edit.SetSchool(this.Select_school);
                    if(dancer_edit.EventId == this.Id_event) dancer_edit.MemberNum = Convert.ToInt32(this.MemberNum);
                }

                if (dancer_group != null) {
                    foreach (MemberGroup group_edit in event_edit.Groups)
                    {
                        if (group_edit.GroupMembers.Contains(dancer_group)) group_edit.UpdateGroupMembersString();
                    }
                }
            }

            

            await DanceRegDatabase.ExecuteNonQueryAsync("update dancers set Firstname='" + this.Firstname + "', Surname='" + this.Surname + "', Id_school=" + this.Select_school.Id + " where Id_member=" + this.Id_member);
            if (this.Id_event > 0) await DanceRegDatabase.ExecuteNonQueryAsync("update nums_for_members set Number=" + this.MemberNum + " where Id_event=" + this.Id_event + " and Id_member=" + this.Id_member + " and Is_group=0");

            base.Command_save?.Execute();
        }

        public RelayCommand Command_AddSchool
        {
            get => new RelayCommand(obj =>
            {
                AddSchoolView window = new AddSchoolView();
                if ((bool)window.ShowDialog())
                {
                    this.OnPropertyChanged("Schools");
                    this.Select_school = this.Schools.Last();
                }
            });
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveDancerMethod();
            });
        }
    }
}
