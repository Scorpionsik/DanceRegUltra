using CoreWPF.Utilites.Navigation;
using CoreWPF.Windows;
using DanceRegUltra.Enums;
using DanceRegUltra.ViewModels;
using DanceRegUltra.ViewModels.CategoryMenuElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DanceRegUltra.Views
{
    /// <summary>
    /// Логика взаимодействия для CategoryManagerViewModel.xaml
    /// </summary>
    public partial class CategoryManagerView : DialogWindowExt
    {
        public CategoryManagerView(CategoryType select_type = CategoryType.League)
        {
            InitializeComponent();
            NavigationManager nav = new NavigationManager(this.Dispatcher, this.Frame);
            nav.Register<LeagueMenuElementViewModel, CategoryMenuElementView>(new LeagueMenuElementViewModel(nav), CategoryType.League.ToString());
            nav.Register<AgeMenuElementViewModel, CategoryMenuElementView>(new AgeMenuElementViewModel(nav), CategoryType.Age.ToString());
            nav.Register<StyleMenuElementViewModel, CategoryMenuElementView>(new StyleMenuElementViewModel(nav), CategoryType.Style.ToString());
            this.DataContext = new CategoryManagerViewModel(select_type, nav);
        }
    }
}
