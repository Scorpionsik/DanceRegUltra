using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Models;
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
            this.Initialize();
        }

        public override WindowClose CloseMethod()
        {
            if(this.JsonEdit == null) DanceRegCollections.ClearCategories();
            return base.CloseMethod();
        }

        private async void Initialize()
        {
            DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from leagues where IsHide=0");
            foreach(DbRow row in res)
            {
                DanceRegCollections.LoadLeague(new Models.CategoryString(row["Id_league"].ToInt32(), Models.CategoryType.League, row["Name"].ToString(), row["Position"].ToInt32()));
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from ages where IsHide=0");
            foreach (DbRow row in res)
            {
                DanceRegCollections.LoadAge(new Models.CategoryString(row["Id_age"].ToInt32(), Models.CategoryType.Age, row["Name"].ToString(), row["Position"].ToInt32()));
            }

            res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from styles where IsHide=0");
            foreach (DbRow row in res)
            {
                DanceRegCollections.LoadStyle(new Models.CategoryString(row["Id_style"].ToInt32(), Models.CategoryType.Style, row["Name"].ToString(), row["Position"].ToInt32()));
            }

            this.Schemes = new ListExt<DanceScheme>();
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
