using CoreWPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.Categories
{
    public class IdCheck : NotifyPropertyChanged
    {
        private event Action event_UpdateCheck;
        public event Action Event_UpdateCheck
        {
            add
            {
                this.event_UpdateCheck -= value;
                this.event_UpdateCheck += value;
            }
            remove => this.event_UpdateCheck -= value;
        }

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
                this.event_UpdateCheck?.Invoke();
            }
        }

        public IdCheck(int id, bool isChecked = false)
        {
            this.Id = id;
            this.isChecked = isChecked;
        }
    }
}
