using CoreWPF.MVVM;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class EventManagerViewModel : ViewModel
    {
        private DanceEvent EventInWork;

        public EventManagerViewModel(int idEventLoad)
        {
            this.EventInWork = DanceRegCollections.GetEventById(idEventLoad);
            this.Title = "["+ this.EventInWork.Title +"] Менеджер событий - " + App.AppTitle;
        }

        public override WindowClose CloseMethod()
        {
            DanceRegCollections.UnloadEvent(this.EventInWork);
            return base.CloseMethod();
        }
    }
}
