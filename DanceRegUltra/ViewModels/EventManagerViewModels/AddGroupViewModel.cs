﻿using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Utilites;
using DanceRegUltra.Utilites.Converters;
using DanceRegUltra.Views.EventManagerViews;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class AddGroupViewModel : ViewModel
    {
        private bool enableAddButton;
        public bool EnableAddButton
        {
            get => this.enableAddButton;
            private set
            {
                this.enableAddButton = value;
                this.OnPropertyChanged("EnableAddButton");
            }
        }
        public bool IsEnableDancerEdit
        {
            get => this.DancerInWork?.MemberId > 0 ? false : true;
        }

        public bool IsEnableGroupEdit
        {
            get => this.Select_group?.MemberId > 0 ? false : true;
        }

        public DanceEvent EventInWork { get; private set; }

        public FindDancer FindList { get; private set; }

        public ListExt<MemberGroup> Groups { get; private set; }

        private MemberGroup select_group;
        public MemberGroup Select_group
        {
            get => this.select_group;
            set
            {
                this.select_group = value;
                this.Select_dancer = null;
                this.Select_school = value == null || value.School == null ? null : DanceRegCollections.GetSchoolById(value.School.Id);
                this.OnPropertyChanged("Select_group");
                this.OnPropertyChanged("IsEnableGroupEdit");
            }
        }

        private MemberDancer select_dancer;
        public MemberDancer Select_dancer
        {
            get => this.select_dancer;
            set
            {
                this.select_dancer = value;
                this.OnPropertyChanged("Select_dancer");
            }
        }

        private MemberDancer dancerInWork;
        public MemberDancer DancerInWork
        {
            get => this.dancerInWork;
            private set
            {
                this.dancerInWork = value;
                this.OnPropertyChanged("DancerInWork");
                this.OnPropertyChanged("DancerName");
                this.OnPropertyChanged("DancerSurname");
                this.OnPropertyChanged("IsEnableDancerEdit");
            }
        }

        public string DancerName
        {
            get => this.DancerInWork.Name;
            set
            {
                this.DancerInWork.SetName(value);
                this.OnPropertyChanged("DancerName");
                this.FindList.Find(this.DancerInWork.Name, this.DancerInWork.Surname);
            }
        }

        public string DancerSurname
        {
            get => this.DancerInWork.Surname;
            set
            {
                this.DancerInWork.SetSurname(value);
                this.OnPropertyChanged("DancerSurname");
                this.FindList.Find(this.DancerInWork.Name, this.DancerInWork.Surname);
            }
        }

        private IdTitle select_platform;
        public IdTitle Select_platform
        {
            get => this.select_platform;
            private set
            {
                this.select_platform = value;
                this.OnPropertyChanged("Select_platform");
            }
        }

        private KeyValuePair<int, List<IdTitle>> select_league;
        public KeyValuePair<int, List<IdTitle>> Select_league
        {
            get => this.select_league;
            set
            {
                this.select_league = value;
                this.OnPropertyChanged("Select_league");
                this.SetSchemeType(SchemeType.Platform, value.Value);
            }
        }

        private IdTitle select_block;
        public IdTitle Select_block
        {
            get => this.select_block;
            private set
            {
                this.select_block = value;
                this.OnPropertyChanged("Select_block");
            }
        }

        private KeyValuePair<int, List<IdTitle>> select_age;
        public KeyValuePair<int, List<IdTitle>> Select_age
        {
            get => this.select_age;
            set
            {
                this.select_age = value;
                this.OnPropertyChanged("Select_age");
                this.SetSchemeType(SchemeType.Block, value.Value);
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

        public List<IdCheck> Styles { get; private set; }

        private string showSelectStyles;
        public string ShowSelectStyles
        {
            get => this.showSelectStyles;
            set
            {
                this.showSelectStyles = value;
                this.OnPropertyChanged("ShowSelectStyles");
            }
        }

        private string comboBoxTextStyle;
        public string ComboBoxTextStyle
        {
            get => this.comboBoxTextStyle;
            set
            {
                this.comboBoxTextStyle = "";
                this.ShowSelectStyles = "";
                if (this.Styles == null) this.comboBoxTextStyle += "0";
                else
                {
                    foreach (IdCheck style in this.Styles)
                    {
                        if (style.IsChecked)
                        {
                            this.ShowSelectStyles += CategoryNameByIdConvert.Convert(style.Id, CategoryType.Style) + "\r\n";
                        }
                    }
                    if (this.ShowSelectStyles.Length - 2 >= 0) this.ShowSelectStyles = this.ShowSelectStyles.Remove(this.ShowSelectStyles.Length - 2);
                }
                this.OnPropertyChanged("ComboBoxTextStyle");
            }
        }
        public AddGroupViewModel(int event_id, MemberGroup group = null) : base()
        {
            this.EnableAddButton = true;
            this.DancerInWork = new MemberDancer(-1, -1, "", "");
            this.ComboBoxTextStyle = "";
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.Groups = new ListExt<MemberGroup>();

            MemberGroup tmp_group = new MemberGroup(this.EventInWork.IdEvent, -1, new List<MemberDancer>());
            this.Groups.Add(tmp_group);
            this.Groups.AddRange(this.EventInWork.Groups);
            if (group == null) this.Select_group = tmp_group;
            else this.Select_group = group;

            this.FindList = new FindDancer(-1, 1000);
            this.FindList.Event_changeSelectDancer += this.SetDancerFromSearch;
            this.FindList.Event_FinishSearch += this.UpdateFindList;

            this.Styles = new List<IdCheck>();
            foreach (int style in this.EventInWork.Styles)
            {
                this.Styles.Add(new IdCheck(style));
            }
            this.Title = "[" + this.EventInWork.Title + "] Добавить новую группу - " + App.AppTitle;
        }

        public AddGroupViewModel(DanceNomination nomination) : this(nomination.Event_id, null)
        {
            this.Select_block = nomination.Block_info;

            foreach (KeyValuePair<int, List<IdTitle>> league in this.EventInWork.Leagues)
            {
                if (league.Key == nomination.League_id)
                {
                    this.Select_league = league;
                    break;
                }
            }

            foreach (KeyValuePair<int, List<IdTitle>> age in this.EventInWork.Ages)
            {
                if (age.Key == nomination.Age_id)
                {
                    this.select_age = age;
                    break;
                }
            }
        }

        public void CheckStyle(int style_id)
        {
            if (style_id > 0)
            {
                foreach (IdCheck style in this.Styles)
                {
                    if (style.Id == style_id)
                    {
                        style.IsChecked = true;
                        break;
                    }
                }
            }
        }

        private void UpdateFindList()
        {
            this.DancerInWork.SetName(App.CapitalizeAllWords(this.DancerName));
            this.OnPropertyChanged("DancerName");
            this.DancerInWork.SetSurname(App.CapitalizeAllWords(this.DancerSurname));
            this.OnPropertyChanged("DancerSurname");
            this.OnPropertyChanged("FindList");
        }
       
        private void SetDancerFromSearch(MemberDancer dancer)
        {
            this.DancerInWork = dancer;
            //if(this.Select_group != null) this.Select_school = this.Select_group.School;
        }

        private void SetSchemeType(SchemeType type, IEnumerable<IdTitle> values)
        {

            switch (type)
            {
                case SchemeType.Platform:
                    if (values == null) this.Select_platform = null;
                    else
                    {
                        if (values.Count() == 1) this.Select_platform = values.ElementAt(0);
                        else
                        {
                            SelectSchemeTypeView window = new SelectSchemeTypeView(this.Select_league.Key, SchemeType.Platform, values, this.Select_platform);
                            if ((bool)window.ShowDialog())
                            {
                                this.Select_platform = window.Select_value;
                            }

                        }
                    }
                    break;
                case SchemeType.Block:
                    if (values == null) this.Select_block = null;
                    else
                    {
                        if (values.Count() == 1) this.Select_block = values.ElementAt(0);
                        else
                        {
                            SelectSchemeTypeView window = new SelectSchemeTypeView(this.Select_age.Key, SchemeType.Block, values, this.Select_block);
                            if ((bool)window.ShowDialog())
                            {
                                this.Select_block = window.Select_value;
                            }
                        }
                    }
                    break;

            }

        }
        
        private async void AddNodeMethod()
        {
            this.EnableAddButton = false;
            MemberGroup tmp_group = null;

            if (this.IsEnableGroupEdit)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into groups (Json_members, Id_school) values ('" + this.Select_group.GetMembers() + "', " + this.Select_school.Id + ")");
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select groups.Id_member, groups.Json_members, groups.Id_school, schools.Name from groups join schools using (Id_school) order by groups.Id_member");
                DbRow row = res.GetRow(res.RowsCount - 1);
                tmp_group = new MemberGroup(this.EventInWork.IdEvent, row.GetInt32("Id_member"), row["Json_members"].ToString());
                tmp_group.SetSchool(DanceRegCollections.GetSchoolById(row.GetInt32("Id_school")));

                this.Groups.Insert(0, new MemberGroup(this.EventInWork.IdEvent, -1, new List<MemberDancer>()));
                int update_insert = this.Groups.IndexOf(this.Select_group);
                this.Groups[update_insert] = tmp_group;
                this.Select_group = tmp_group;
                await this.EventInWork.AddMember(tmp_group);
            }
            else
            {
                tmp_group = this.Select_group;
            }
            int update_position = -1, tmp_position = -1;
            foreach (IdCheck style in this.Styles)
            {
                if (style.IsChecked)
                {
                    tmp_position = await this.EventInWork.AddNodeAsync(tmp_group, true, this.Select_platform, this.Select_league.Key, this.Select_block, this.Select_age.Key, style.Id);
                    if (tmp_position > -1 && (update_position == -1 || update_position > tmp_position)) update_position = tmp_position;
                }
            }
            if (update_position > -1) await this.EventInWork.UpdateNodePosition(update_position, this.EventInWork.Nodes.Count - 1, true);
            this.EnableAddButton = true;
        }

        private async void AddMemberInGroupMethod()
        {
            MemberDancer tmp_dancer = DanceRegCollections.GetGroupDancerById(this.DancerInWork.MemberId); 

            if (tmp_dancer == null)
            {
                if (this.DancerInWork.MemberId == -1)
                {
                    await DanceRegDatabase.ExecuteNonQueryAsync("insert into dancers (Firstname, Surname, Id_school) values ('" + this.DancerName + "', '" + this.DancerSurname + "', " + this.Select_school.Id + ")");
                    DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select dancers.Id_member, dancers.Firstname, dancers.Surname, dancers.Id_school, schools.Name from dancers join schools using (Id_school) order by dancers.Id_member");
                    DbRow row = res.GetRow(res.RowsCount - 1);
                    tmp_dancer = new MemberDancer(-1, row.GetInt32("Id_member"), row["Firstname"].ToString(), row["Surname"].ToString());
                    tmp_dancer.SetSchool(DanceRegCollections.GetSchoolById(row.GetInt32("Id_school")));
                }
                else tmp_dancer = this.DancerInWork;
                DanceRegCollections.AddGroupDancer(tmp_dancer);
            }

            this.Select_group.AddMember(tmp_dancer);
            this.Command_ClearMemberInGroup.Execute();
            this.OnPropertyChanged("Groups");
            this.OnPropertyChanged("Select_group");
            
        }

        public RelayCommand Command_ChangePlatform
        {
            get => new RelayCommand(obj =>
            {
                this.SetSchemeType(SchemeType.Platform, this.Select_league.Value);
            },
                (obj) => this.Select_league.Value != null && this.Select_league.Value.Count > 1);
        }

        public RelayCommand Command_ChangeBlock
        {
            get => new RelayCommand(obj =>
            {
                this.SetSchemeType(SchemeType.Block, this.Select_age.Value);
            },
                (obj) => this.Select_age.Value != null && this.Select_age.Value.Count > 1);
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

        public RelayCommand Command_AddNode
        {
            get => new RelayCommand(obj =>
            {
                this.AddNodeMethod();
            },
                (obj) => this.Select_group.GroupMembers.Count >= 2 && this.Select_league.Key > 0 && this.Select_age.Key > 0 && this.ShowSelectStyles.Length > 0 && this.Select_block != null && this.Select_school != null);
        }

        public RelayCommand Command_AddMemberInGroup
        {
            get => new RelayCommand(obj =>
            {
                this.AddMemberInGroupMethod();
            },
                (obj) => this.Select_school != null && this.DancerName != null && this.DancerName.Length > 0 && this.DancerSurname != null && this.DancerSurname.Length > 0);
        }

        public RelayCommand Command_ClearMemberInGroup
        {
            get => new RelayCommand(obj =>
            {
                this.FindList.Clear();

                this.SetDancerFromSearch(new MemberDancer(-1, -1, "", ""));
            });
        }

        public RelayCommand<MemberDancer> Command_DeleteMemberInGroup
        {
            get => new RelayCommand<MemberDancer>(dancer =>
            {
                this.Select_group.RemoveMember(dancer);
                this.OnPropertyChanged("Groups");
                this.OnPropertyChanged("Select_group");
            },
                (dancer) => dancer != null);
        }
    }
}
