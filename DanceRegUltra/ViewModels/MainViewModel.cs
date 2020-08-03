using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public StatusString Status { get; private set; }

        public MainViewModel() : base()
        {
            this.Title = App.AppTitle + ": Создание или выбор события";

            this.Status = new StatusString();
            this.Initialize();
        }

        private async void Initialize()
        {
            await Task.Run(() =>
            {
                //загрузить события
            });
        }
    }
}
