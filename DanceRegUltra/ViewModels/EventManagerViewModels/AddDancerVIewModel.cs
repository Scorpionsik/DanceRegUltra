using CoreWPF.MVVM;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class AddDancerViewModel : ViewModel
    {
        private int EventID;
        
        internal FindDancer FindList { get; private set; }

        private MemberDancer dancerInWork;
        public MemberDancer DancerInWork
        {
            get => this.dancerInWork;
            private set
            {
                this.dancerInWork = value;
                this.OnPropertyChanged("DancerInWork");
            }
        }

        public List<int> Leagues { get; private set; }
        public List<int> Ages { get; private set; }
        public List<IdCheck> Styles { get; private set; }

        public AddDancerViewModel(int event_id)
        {
            this.EventID = event_id;
            this.DancerInWork = new MemberDancer(event_id, -1, "", "");
        }
  
    }
}
