using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Views;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DanceRegUltra.ViewModels
{
    public class SchemeManagerViewModel : ViewModel
    {

        private event Action<DanceScheme> event_SetReturnScheme;
        public event Action<DanceScheme> Event_SetReturnScheme
        {
            add
            {
                this.event_SetReturnScheme -= value;
                this.event_SetReturnScheme += value;
            }
            remove => this.event_SetReturnScheme -= value;
        }
        public bool EventMode { get; private set; }
        
        public static bool IsSchemeManagerExist { get; private set; }

        private bool updateConfirm;
        public bool UpdateConfirm
        {
            get
            {
                if(this.JsonEdit == null)
                {
                    return this.updateConfirm;
                }
                else
                {
                    return this.Select_scheme != null;
                }
            }
            private set
            {
                this.updateConfirm = value;
                this.OnPropertyChanged("UpdateConfirm");
            }
        }

        private string jsonEdit;
        public string JsonEdit
        {
            get => this.jsonEdit;
            set
            {
                this.jsonEdit = value;
                this.OnPropertyChanged("JsonEdit");
                this.OnPropertyChanged("ContentSaveButton");
            }
        }

        public string ContentSaveButton
        {
            get
            {
                switch (this.EventMode)
                {
                    case true:
                        if (this.Select_scheme.UpdateFlag) return "Выбрать схему и сохранить изменения";
                        else return "Выбрать схему";
                       
                    case false:
                        return "Сохранить изменения";
                     
                }
                return "";
                /*
                if (this.JsonEdit == null) return "Сохранить изменения";
                else
                {
                    if(this.Select_scheme != null)
                    {
                        if (this.Select_scheme.UpdateFlag) return "Выбрать схему и сохранить изменения";
                    }
                    return "Выбрать схему";
                }*/


            }
        }

        public static List<IdCheck> AllLeagues { get; private set; }
        public static List<IdCheck> AllAges { get; private set; }
        public static List<IdCheck> AllStyles { get; private set; }

        public ListExt<DanceScheme> Schemes { get; private set; }

        private List<int> DeleteschemesId;

        private DanceScheme select_scheme;
        public DanceScheme Select_scheme
        {
            get => this.select_scheme;
            set
            {
                this.select_scheme = value;
                this.OnPropertyChanged("Select_scheme");
                this.OnPropertyChanged("Title");
                this.OnPropertyChanged("UpdateConfirm");
            }
        }

        public override string Title
        {
            get
            {
                if (this.Select_scheme != null) return base.Title + " ["+ this.Select_scheme.Title_scheme +"]";
                else return base.Title;
            }
            set => base.Title = value;
        }

        static SchemeManagerViewModel()
        {
            IsSchemeManagerExist = false;
        }

        private SchemeManagerViewModel() : base()
        {
            IsSchemeManagerExist = true;

            this.Title = "Редактор шаблонов схем - " + App.AppTitle;
            //this.JsonEdit = jsonEdit;
            this.DeleteschemesId = new List<int>();

            AllLeagues = new List<IdCheck>();
            AllAges = new List<IdCheck>();
            AllStyles = new List<IdCheck>();

            this.Schemes = new ListExt<DanceScheme>();
            this.Schemes.CollectionChanged += this.UpdateDanceSchemes;
            
        }

        public SchemeManagerViewModel(string jsonEdit) : this()
        {
            if (jsonEdit != null && jsonEdit.Length > 0) this.EventMode = true;
            else this.EventMode = false;
            this.Initialize(jsonEdit);
        }

        public SchemeManagerViewModel(int insert_index) : this()
        {
            this.EventMode = true;
            this.Initialize("", insert_index);
        }

        public override WindowClose CloseMethod()
        {
            this.event_SetReturnScheme?.Invoke(this.Select_scheme);
            DanceRegCollections.ClearCategories();
            IsSchemeManagerExist = false;
            return base.CloseMethod();
        }

        private async void SaveChangesMethod()
        {
            foreach (DanceScheme scheme in this.Schemes)
            {
                if (scheme.UpdateFlag)
                {
                    string json = JsonScheme.Serialize(new JsonScheme(scheme));
                    if (scheme.Id_scheme == -1) await DanceRegDatabase.ExecuteNonQueryAsync("insert into 'template_schemes' ('Title', 'Json_scheme') values ('" + scheme.Title_scheme + "', '" + json + "')");
                    else await DanceRegDatabase.ExecuteNonQueryAsync("update 'template_schemes' set 'Title'='" + scheme.Title_scheme + "', 'Json_scheme'='" + json + "' where Id_scheme=" + scheme.Id_scheme);  
                }
            }

            foreach(int delete_id in this.DeleteschemesId)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("delete from 'template_schemes' where Id_scheme=" + delete_id);
            }
            base.Command_save?.Execute();
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveChangesMethod();
            },
                (obj) => this.UpdateConfirm);
        }

        private async void Initialize(string jsonEdit, int select_index = -1)
        {
            this.JsonEdit = jsonEdit;

            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from leagues");
            IdCheck[] tmp_array = new IdCheck[res.RowsCount];
            int insert_index = -1;
            foreach(DbRow row in res)
            {
                insert_index = DanceRegCollections.LoadLeague(new CategoryString(row["Id_league"].ToInt32(), CategoryType.League, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean()));
                tmp_array[insert_index] = new IdCheck(row["Id_league"].ToInt32());
            }
            
            for(int i = 0; i < DanceRegCollections.Leagues.Value.Count; i++)
            {
                if (!DanceRegCollections.Leagues.Value[i].IsHide) AllLeagues.Add(tmp_array[i]);
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from ages");
            tmp_array = new IdCheck[res.RowsCount];
            foreach (DbRow row in res)
            {
                insert_index = DanceRegCollections.LoadAge(new CategoryString(row["Id_age"].ToInt32(), CategoryType.Age, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean()));
                tmp_array[insert_index] = new IdCheck(row["Id_age"].ToInt32());
            }

            for (int i = 0; i < DanceRegCollections.Ages.Value.Count; i++)
            {
                if (!DanceRegCollections.Ages.Value[i].IsHide) AllAges.Add(tmp_array[i]);
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from styles");
            tmp_array = new IdCheck[res.RowsCount];
            foreach (DbRow row in res)
            {
                insert_index = DanceRegCollections.LoadStyle(new CategoryString(row["Id_style"].ToInt32(), CategoryType.Style, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean()));
                tmp_array[insert_index] = new IdCheck(row["Id_style"].ToInt32());

            }

            for (int i = 0; i < DanceRegCollections.Styles.Value.Count; i++)
            {
                if (!DanceRegCollections.Styles.Value[i].IsHide) AllStyles.Add(tmp_array[i]);
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from template_schemes");
            foreach(DbRow row in res)
            {
                this.Schemes.Add(new DanceScheme(row["Id_scheme"].ToInt32(), row["Title"].ToString(), JsonScheme.Deserialize(row["Json_scheme"].ToString())));
                if (select_index > 0 && select_index == row["Id_scheme"].ToInt32()) this.Select_scheme = this.Schemes.Last;
            }

            if (this.Schemes.Count == 0) this.Schemes.Add(new DanceScheme());
            if(select_index < 1) this.Select_scheme = this.Schemes.First;
        }

        private void UpdateDanceSchemes(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    ((DanceScheme)e.NewItems[0]).Event_UpdateDanceScheme += this.ChangeUpdateFlag;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((DanceScheme)e.OldItems[0]).Event_UpdateDanceScheme += this.ChangeUpdateFlag;
                    break;
            }
        }

        private void ChangeUpdateFlag(bool isUpdateTitleScheme)
        {
            if(!this.updateConfirm) this.UpdateConfirm = true;
            if (isUpdateTitleScheme) this.OnPropertyChanged("Title");
        }

        public RelayCommand Command_AddNewScheme
        {
            get => new RelayCommand(obj =>
            {
                this.Schemes.Add(new DanceScheme("Схема " + (this.Schemes.Count + 1)));
                this.ChangeUpdateFlag(false);
            });
        }

        public RelayCommand<DanceScheme> Command_CopySelectScheme
        {
            get => new RelayCommand<DanceScheme>(scheme =>
            {
                DanceScheme copy_scheme = new DanceScheme(-1, scheme.Title_scheme + "_копия", new JsonScheme(scheme), true);
                this.Schemes.Add(copy_scheme);
                this.Select_scheme = copy_scheme;
                this.ChangeUpdateFlag(false);
            });
        }

        public RelayCommand<DanceScheme> Command_DeleteSelectScheme
        {
            get => new RelayCommand<DanceScheme>(scheme =>
            {
                if (scheme.Id_scheme != -1) this.DeleteschemesId.Add(scheme.Id_scheme);
                this.Schemes.Remove(scheme);
                if (this.Schemes.Count == 0) this.Schemes.Add(new DanceScheme());
                this.Select_scheme = this.Schemes.Last;
                this.ChangeUpdateFlag(false);
            });
        }

        public static RelayCommand<string> Command_OpencategoryWindow
        {
            get => new RelayCommand<string>(param =>
            {
                CategoryType type = CategoryType.League;
                switch (param)
                {
                    case "league":
                        type = CategoryType.League;
                        break;
                    case "age":
                        type = CategoryType.Age;
                        break;
                    case "style":
                        type = CategoryType.Style;
                        break;
                }

                CategoryManagerView window = new CategoryManagerView(type);
                window.Show();
            });
        }
    }
}
