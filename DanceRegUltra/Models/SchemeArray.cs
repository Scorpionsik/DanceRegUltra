using CoreWPF.Utilites;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models
{
    public struct SchemeArray : INotifyPropertyChanged
    {
        private string titleSchemePart;
        public string TitleSchemePart
        {
            get => this.titleSchemePart;
            set
            {
                this.titleSchemePart = value;
                this.OnPropertyChanged("TitleSchemePart");
            }
        }

        public ListExt<int> SchemePartValues { get; private set; }

        public SchemeArray(string title)
        {
            this.titleSchemePart = title;
            this.SchemePartValues = new ListExt<int>();
            this.PropertyChanged = null;
        }
        
        /// <summary>
        /// Событие для обновления привязанного объекта (в XAML)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Метод для обновления выбранного привязанного объекта (в XAML)
        /// </summary>
        /// <param name="prop">Принимает строку-имя объекта, который необходимо обновить</param>
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        //---метод OnPropertyChanged
    }
}
