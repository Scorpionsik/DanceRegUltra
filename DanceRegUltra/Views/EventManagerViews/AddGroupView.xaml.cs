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
    /// Логика взаимодействия для AddGroupView.xaml
    /// </summary>
    public partial class AddGroupView : DialogWindowExt
    {
        private int style_id = -1;

        private AddGroupView()
        {
            InitializeComponent();
        }

        public AddGroupView(int event_id) : this()
        {
            this.DataContext = new AddGroupViewModel(event_id);
        }

        public AddGroupView(int event_id, MemberGroup group) : this()
        {
            this.DataContext = new AddGroupViewModel(event_id, group);
        }

        public AddGroupView(DanceNomination nomination) : this()
        {
            this.DataContext = new AddGroupViewModel(nomination);
            this.style_id = nomination.Style_id;
        }

        private void DialogWindowExt_Loaded(object sender, RoutedEventArgs e)
        {
            ((AddGroupViewModel)this.DataContext).CheckStyle(this.style_id);
        }
    }
}
