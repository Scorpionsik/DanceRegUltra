using CoreWPF.Windows;
using DanceRegUltra.Models;
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
    /// Логика взаимодействия для RegisterWindowView.xaml
    /// </summary>
    public partial class RegisterWindowView : DialogWindowExt
    {
        public DanceEvent Return_event { get; private set; }

        public RegisterWindowView()
        {
            InitializeComponent();
            RegisterWindowViewModel vm = new RegisterWindowViewModel();
            vm.Event_SetReturnEvent += this.SetReturnEvent;
            this.DataContext = vm;
        }

        private void SetReturnEvent(DanceEvent return_event)
        {
            this.Return_event = return_event;
        }
    }
}
