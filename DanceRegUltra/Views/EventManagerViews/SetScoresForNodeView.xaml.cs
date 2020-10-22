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
    /// Логика взаимодействия для SetScoresForNodeView.xaml
    /// </summary>
    public partial class SetScoresForNodeView : DialogWindowExt
    {
        public SetScoresForNodeView(int event_id, DanceNode node)
        {
            InitializeComponent();
            this.DataContext = new SetScoresForNodeViewModel(event_id, node);
        }
    }
}
