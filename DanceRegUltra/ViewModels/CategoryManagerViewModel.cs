using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Utilites.Navigation;
using CoreWPF.Utilites.Navigation.Interfaces;
using CoreWPF.Windows.Enums;
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

namespace DanceRegUltra.ViewModels
{
    public class CategoryManagerViewModel : NavigationViewModel
    {
        public List<CategoryType> MenuItems { get; private set; }

        private string name_category;
        public string Name_category
        {
            get => this.name_category;
            set
            {
                this.name_category = value;
                this.OnPropertyChanged("Name_category");
            }
        }

        private CategoryType select_menu;
        public CategoryType Select_menu
        {
            get => this.select_menu;
            set
            {
                this.select_menu = value;
                this.OnPropertyChanged("Select_menu");
                this.Navigator.Navigate(this.select_menu.ToString());
            }
        }

        public CategoryManagerViewModel(CategoryType select_menu, NavigationManager nav) : base(nav)
        {
            this.Title = "Редактор категорий - " + App.AppTitle;
            this.MenuItems = new List<CategoryType>(Enum.GetValues(typeof(CategoryType)).Cast<CategoryType>());
            this.Select_menu = select_menu;
            this.Initialize();
        }

        private async void Initialize()
        {
            if (!SchemeManagerViewModel.IsSchemeManagerExist)
            {
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from leagues");
                foreach(DbRow row in res)
                {
                    CategoryString add_league = new CategoryString(row["Id_league"].ToInt32(), CategoryType.League, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean());
                    DanceRegCollections.LoadLeague(add_league);
                }

                res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from ages");
                foreach (DbRow row in res)
                {
                    CategoryString add_age = new CategoryString(row["Id_age"].ToInt32(), CategoryType.Age, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean());
                    DanceRegCollections.LoadAge(add_age);
                }

                res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select * from styles");
                foreach (DbRow row in res)
                {
                    CategoryString add_style = new CategoryString(row["Id_style"].ToInt32(), CategoryType.Style, row["Name"].ToString(), row["Position"].ToInt32(), row["IsHide"].ToBoolean());
                    DanceRegCollections.LoadStyle(add_style);
                }
            }
        }

        public override WindowClose CloseMethod()
        {
            if (!SchemeManagerViewModel.IsSchemeManagerExist) DanceRegCollections.ClearCategories();
            return base.CloseMethod();
        }

        public override void SetContent(INavigateModule module)
        {
            if (module is ICategoriesMenu command)
            {
                this.Command_add = command.Command_add;
                this.Command_remove = command.Command_remove;
            }
        }

        private RelayCommand<string> command_add;
        public RelayCommand<string> Command_add
        {
            get { return this.command_add; }
            private set
            {
                this.command_add = value;
                this.OnPropertyChanged("Command_add");
            }
        }

        private RelayCommand command_remove;
        public RelayCommand Command_remove
        {
            get { return this.command_remove; }
            private set
            {
                this.command_remove = value;
                this.OnPropertyChanged("Command_remove");
            }
        }
    }
}
