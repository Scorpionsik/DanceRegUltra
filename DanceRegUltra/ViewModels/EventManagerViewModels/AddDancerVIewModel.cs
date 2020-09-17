using CoreWPF.MVVM;
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class AddDancerViewModel : ViewModel
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
        public DanceEvent EventInWork { get; private set; }

        public FindDancer FindList { get; private set; }

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

        public ListExt<IdCheck> Styles { get; private set; }

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
                    if(this.ShowSelectStyles.Length - 2 >= 0) this.ShowSelectStyles = this.ShowSelectStyles.Remove(this.ShowSelectStyles.Length - 2);
                }
                this.OnPropertyChanged("ComboBoxTextStyle");
            }
        }

        public AddDancerViewModel(int event_id, MemberDancer dancer = null)
        {
            
            this.EnableAddButton = true;
            this.ComboBoxTextStyle = "";
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.SetDancerFromSearch(dancer == null ? new MemberDancer(event_id, -1, "", "") : dancer);
            
            this.FindList = new FindDancer(1000);
            this.FindList.Event_changeSelectDancer += this.SetDancerFromSearch;
            this.FindList.Event_FinishSearch += this.UpdateFindList;
            this.Styles = new ListExt<IdCheck>();
            foreach(int style in this.EventInWork.Styles)
            {
                this.Styles.Add(new IdCheck(style));
            }
            this.Title = "[" + this.EventInWork.Title + "] Добавить нового танцора - " + App.AppTitle;
        }

        public AddDancerViewModel(DanceNomination nomination) : this(nomination.Event_id, null)
        {
            this.Select_block = nomination.Block_info;

            foreach(KeyValuePair<int, List<IdTitle>> league in this.EventInWork.Leagues)
            {
                if (league.Key == nomination.League_id)
                {
                    this.Select_league = league;
                    break;
                }
            }

            foreach(KeyValuePair<int, List<IdTitle>> age in this.EventInWork.Ages)
            {
                if(age.Key == nomination.Age_id)
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
            this.Select_school = this.DancerInWork.School;
            /*
            foreach (IdTitle school in DanceRegCollections.Schools.Value)
            {
                if(school.Id == dancer.School.Id)
                {
                    this.Select_school = school;
                    break;
                }
            }*/
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
            MemberDancer tmp_dancer = null;

            if (this.IsEnableDancerEdit)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into dancers (Firstname, Surname, Id_school) values ('" + this.DancerName + "', '" + this.DancerSurname + "', " + this.Select_school.Id + ")");
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select dancers.Id_member, dancers.Firstname, dancers.Surname, dancers.Id_school, schools.Name from dancers join schools using (Id_school) order by dancers.Id_member");
                DbRow row = res[res.RowsCount - 1];
                tmp_dancer = new MemberDancer(this.EventInWork.IdEvent, row["Id_member"].ToInt32(), row["Firstname"].ToString(), row["Surname"].ToString());
                tmp_dancer.SetSchool(DanceRegCollections.GetSchoolById(row["Id_school"].ToInt32()));
                
                this.SetDancerFromSearch(tmp_dancer);
                await this.EventInWork.AddMember(tmp_dancer);
            }
            else
            {
                tmp_dancer = this.EventInWork.GetDancerById(this.DancerInWork.MemberId);
                if (tmp_dancer == null)
                {
                    tmp_dancer = this.DancerInWork;
                    await this.EventInWork.AddMember(tmp_dancer);
                    
                }
            }
            foreach (IdCheck style in this.Styles)
            {
                if(style.IsChecked) await this.EventInWork.AddNodeAsync(tmp_dancer, false, this.Select_platform, this.Select_league.Key, this.Select_block, this.Select_age.Key, style.Id);
            }
            this.EnableAddButton = true;
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
                if((bool)window.ShowDialog())
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
            });
        }

        public RelayCommand Command_ClearDancer
        {
            get => new RelayCommand(obj =>
            {
                this.FindList.Clear();
                
                foreach (IdCheck style in this.Styles)
                {
                    style.IsChecked = false;
                }

                this.SetDancerFromSearch(new MemberDancer(this.EventInWork.IdEvent, -1, "", ""));
            });
        }
    }
}
