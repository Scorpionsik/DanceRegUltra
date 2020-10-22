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
    /// Проверить оценки во всей номинации
    /// </summary>
    public partial class ScoresManagerView : DialogWindowExt
    {
        public ScoresManagerView(int event_id, DanceNomination nomination)
        {
            InitializeComponent();
            this.DataContext = new ScoresManagerViewModel(event_id, nomination);
        }
    }
}
