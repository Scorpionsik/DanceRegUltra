using CoreWPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models
{
    internal class IdCheck : NotifyPropertyChanged
    {
        private int id;
        public int Id
        {
            get => this.id;
            set
            {
                this.id = value;
                this.OnPropertyChanged("Id");
            }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get => this.isChecked;
            set
            {
                this.isChecked = value;
                this.OnPropertyChanged("IsChecked");
            }
        }

        public IdCheck(int id, bool isChecked = false)
        {
            this.Id = id;
            this.IsChecked = isChecked;
        }
    }
}
