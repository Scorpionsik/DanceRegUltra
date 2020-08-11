using CoreWPF.Utilites;
using CoreWPF.Utilites.Navigation;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
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

        public RelayCommand Command_add
        {
            get => new RelayCommand(obj =>
            {

            });
        }

        public RelayCommand Command_remove
        {
            get => new RelayCommand(obj =>
            {

            });
        }

        public override void OnNavigatingFrom()
        {

        }

        public override void OnNavigatingTo(object arg)
        {

        }
    }
}
