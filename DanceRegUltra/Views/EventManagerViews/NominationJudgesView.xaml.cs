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
    /// Логика взаимодействия для NominationJudgesView.xaml
    /// </summary>
    public partial class NominationJudgesView : DialogWindowExt
    {
        private NominationJudgesView()
        {
            InitializeComponent();
        }

        public NominationJudgesView(int event_id) : this()
        {
            
            this.DataContext = new NominationJudgesViewModel(event_id);
        }

        public NominationJudgesView(int event_id, DanceNomination select_nomination) : this()
        {
            this.DataContext = new NominationJudgesViewModel(event_id, select_nomination);
        }
    }
}
