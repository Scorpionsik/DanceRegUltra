using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class SchemeManagerViewModel : ViewModel
    {
        private string jsonEdit;
        public string JsonEdit
        {
            get => this.jsonEdit;
            set
            {
                this.jsonEdit = value;
                this.OnPropertyChanged("JsonEdit");
            }
        }

        public static List<IdCheck> AllLeagues { get; private set; }
        public static List<IdCheck> AllAges { get; private set; }
        public static List<IdCheck> AllStyles { get; private set; }

        public ListExt<DanceScheme> Schemes { get; private set; }

        private DanceScheme select_scheme;
        public DanceScheme Select_scheme
        {
            get => this.select_scheme;
            set
            {
                this.select_scheme = value;
                this.OnPropertyChanged("Select_scheme");
            }
        }


        
        public SchemeManagerViewModel(string jsonEdit = null) : base()
        {
            this.Title = App.AppTitle + ": Редактор шаблонов схем";
            this.JsonEdit = jsonEdit;

            AllLeagues = new List<IdCheck>();
            AllAges = new List<IdCheck>();
            AllStyles = new List<IdCheck>();

            this.Schemes = new ListExt<DanceScheme>();

            this.Initialize(jsonEdit);
        }

        public override WindowClose CloseMethod()
        {
            if(this.JsonEdit == null) DanceRegCollections.ClearCategories();
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
            base.Command_save?.Execute();
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveChangesMethod();
            });
        }

        private async void Initialize(string jsonEdit)
        {
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from leagues");
            int insert_index = -1;
            foreach(DbRow row in res)
            {
                insert_index = DanceRegCollections.LoadLeague(new CategoryString(row["Id_league"].ToInt32(), CategoryType.League, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean()));

                if (!row["IsHide"].ToBoolean())
                {
                    AllLeagues.Insert(
                        insert_index,
                        new IdCheck(row["Id_league"].ToInt32())
                        );
                }
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from ages");
            foreach (DbRow row in res)
            {
                insert_index = DanceRegCollections.LoadAge(new CategoryString(row["Id_age"].ToInt32(), CategoryType.Age, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean()));
                if (!row["IsHide"].ToBoolean())
                {
                    AllAges.Insert(
                    insert_index,
                    new IdCheck(row["Id_age"].ToInt32())
                    );
                }
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from styles");
            foreach (DbRow row in res)
            {
                insert_index = DanceRegCollections.LoadStyle(new CategoryString(row["Id_style"].ToInt32(), CategoryType.Style, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean()));
                if (!row["IsHide"].ToBoolean())
                {
                    AllStyles.Insert(
                        insert_index,
                        new IdCheck(row["Id_style"].ToInt32())
                        );
                }
            }

            
            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from template_schemes");
            foreach(DbRow row in res)
            {
                this.Schemes.Add(new DanceScheme(row["Id_scheme"].ToInt32(), row["Title"].ToString(), JsonScheme.Deserialize(row["Json_scheme"].ToString())));
            }

            if (this.Schemes.Count == 0) this.Schemes.Add(new DanceScheme());
            this.Select_scheme = this.Schemes.First;
        }

        public RelayCommand Command_AddNewScheme
        {
            get => new RelayCommand(obj =>
            {
                this.Schemes.Add(new DanceScheme());
            });
        }

        public RelayCommand<DanceScheme> Command_DeleteSelectScheme
        {
            get => new RelayCommand<DanceScheme>(scheme =>
            {
                this.Schemes.Remove(scheme);
                if (this.Schemes.Count == 0) this.Schemes.Add(new DanceScheme());
                this.Select_scheme = this.Schemes.Last;
            });
        }
    }
}
