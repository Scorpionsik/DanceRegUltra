using CoreWPF.Windows;
using DanceRegUltra.ViewModels;
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
    /// Логика взаимодействия для SchemeManagerView.xaml
    /// </summary>
    public partial class SchemeManagerView : DialogWindowExt
    {
        public SchemeManagerView(string jsonEdit = null)
        {
            InitializeComponent();
            this.DataContext = new SchemeManagerViewModel(jsonEdit);
        }
    }
}
