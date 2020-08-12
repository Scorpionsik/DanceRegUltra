using CoreWPF.Utilites;
using CoreWPF.Utilites.Navigation;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.CategoryMenuElements
{
    public class LeagueMenuElementViewModel : NavigationModel, ICategoriesMenu
    {
        public LeagueMenuElementViewModel(NavigationManager nav) : base(CategoryType.League.ToString(), nav)
        {

        }

        public ListExt<CategoryString> Categorys
        {
            get
            {
                ListExt<CategoryString> result = new ListExt<CategoryString>();
                foreach(CategoryString value in DanceRegCollections.Leagues.Value)
                {
                    if (!value.IsHide) result.Add(value);
                }
                return result;
            }
        }

        private CategoryString select_category;

        public CategoryString Select_category
        {
            get => this.select_category;
            set
            {
                this.select_category = value;
                this.OnPropertyChanged("Select_category");
            }
        }

        private async void AddLeagueMethod(string league_name)
        {
            bool isBeginAdd = true;
            foreach(CategoryString league in DanceRegCollections.Leagues.Value)
            {
                if(league.Name == league_name)
                {
                    league.IsHide = false;
                    isBeginAdd = false;
                    break;
                }
            }

            if (isBeginAdd)
            {
                await DanceRegDatabase.ExecuteNonQueryAsync("insert into leagues ('Name') values ('"+ league_name +"')");
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select Id_league, Name from leagues order by Id_league");
                DbRow row = res[res.RowsCount - 1];
                CategoryString add_league = new CategoryString(row["Id_league"].ToInt32(), CategoryType.League, row["Name"].ToString());
                DanceRegCollections.LoadLeague(add_league);
            }
            this.OnPropertyChanged("Categorys");
        }

        public RelayCommand<string> Command_add
        {
            get => new RelayCommand<string>(name =>
            {
                this.AddLeagueMethod(name);
            },
                (name) => name != null && name.Length > 0);
        }

        public RelayCommand Command_remove
        {
            get => new RelayCommand(obj =>
            {
                this.Select_category.IsHide = true;
                this.OnPropertyChanged("Categorys");
            },
                (obj) => this.Select_category != null);
        }

        public override void OnNavigatingFrom() { }

        public override void OnNavigatingTo(object arg) { }
    }
}
