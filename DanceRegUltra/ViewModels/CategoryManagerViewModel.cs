using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Utilites.Navigation;
using CoreWPF.Utilites.Navigation.Interfaces;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class CategoryManagerViewModel : NavigationViewModel
    {
        public List<CategoryType> MenuItems { get; private set; }

        private CategoryType select_menu;
        public CategoryType Select_menu
        {
            get => this.select_menu;
            set
            {
                this.select_menu = value;
                this.OnPropertyChanged("Select_menu");
            }
        }

        public CategoryManagerViewModel(CategoryType select_menu, NavigationManager nav) : base(nav)
        {
            this.Title = "Редактор категорий - " + App.AppTitle;
            this.MenuItems = new List<CategoryType>(Enum.GetValues(typeof(CategoryType)).Cast<CategoryType>());
            this.Select_menu = select_menu;
        }

        public override void SetContent(INavigateModule module)
        {
            if (module is ICategoriesManagerCommands command)
            {
                this.Command_add = command.Command_add;
                this.Command_remove = command.Command_remove;
            }
        }

        private RelayCommand command_add;
        public RelayCommand Command_add
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
