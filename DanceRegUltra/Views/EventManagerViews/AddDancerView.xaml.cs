using CoreWPF.Windows;
using DanceRegUltra.Models;
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
    /// Логика взаимодействия для AddDancerView.xaml
    /// </summary>
    public partial class AddDancerView : DialogWindowExt
    {
        private int style_id = -1;

        private AddDancerView()
        {
            InitializeComponent();
            this.Surname.Focus();
        }

        public AddDancerView(int event_id) : this()
        {
            this.DataContext = new AddDancerViewModel(event_id);
        }

        public AddDancerView(int event_id, MemberDancer dancer) : this()
        {
            this.DataContext = new AddDancerViewModel(event_id, dancer);
        }

        public AddDancerView(DanceNomination nomination) : this()
        {
            this.DataContext = new AddDancerViewModel(nomination);
            this.style_id = nomination.Style_id;
        }

        private void DialogWindowExt_Loaded(object sender, RoutedEventArgs e)
        {
            ((AddDancerViewModel)this.DataContext).CheckStyle(this.style_id);
        }
    }
}
