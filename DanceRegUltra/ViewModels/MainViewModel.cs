using CoreWPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel() : base()
        {
            this.Title = App.AppTitle + ": Создание или выбор события";
        }
    }
}
