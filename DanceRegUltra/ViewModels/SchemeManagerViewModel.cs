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
        private string JsonEdit;

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

        private async void Initialize(string jsonEdit)
        {
            string condition = jsonEdit == null ? " where IsHide=0" : "";
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from leagues" + condition);
            foreach(DbRow row in res)
            {
               AllLeagues.Insert(
                   DanceRegCollections.LoadLeague(new CategoryString(row["Id_league"].ToInt32(), CategoryType.League, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean())),
                   new IdCheck(row["Id_league"].ToInt32())
                   );
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from ages" + condition);
            foreach (DbRow row in res)
            {
                AllAges.Insert(
                    DanceRegCollections.LoadAge(new CategoryString(row["Id_age"].ToInt32(), CategoryType.Age, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean())),
                    new IdCheck(row["Id_age"].ToInt32())
                    );
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from styles" + condition);
            foreach (DbRow row in res)
            {
                AllStyles.Insert(
                    DanceRegCollections.LoadStyle(new CategoryString(row["Id_style"].ToInt32(), CategoryType.Style, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean())),
                    new IdCheck(row["Id_style"].ToInt32())
                    );
            }

            
            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Json_scheme from template_schemes");
            foreach(DbRow row in res)
            {
                //this.Schemes.Add(DanceScheme.Deserialize(res["Json_scheme"].ToString()));
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
