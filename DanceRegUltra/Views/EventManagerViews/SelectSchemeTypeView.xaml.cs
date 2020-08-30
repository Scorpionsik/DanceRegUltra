using CoreWPF.Windows;
using DanceRegUltra.Enums;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.ViewModels.EventManagerViewModels;
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

namespace DanceRegUltra.Views.EventManagerViews
{
    /// <summary>
    /// Логика взаимодействия для SelectSchemeTypeView.xaml
    /// </summary>

    public partial class SelectSchemeTypeView : DialogWindowExt
    {
        public IdTitle Select_value { get; private set; }
        public SelectSchemeTypeView(int category_id, SchemeType type, IEnumerable<IdTitle> values)
        { 
            InitializeComponent();
            SelectSchemeTypeViewModel vm = new SelectSchemeTypeViewModel(category_id, type, values);
            vm.Event_SetTitle += this.SetValue;
            this.DataContext = vm;
        }

        private void SetValue(IdTitle value)
        {
            this.Select_value = value;
        }
    }
}
