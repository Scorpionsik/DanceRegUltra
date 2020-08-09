using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Static;
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
    public enum SchemeType
    {
        Platform,
        Block
    }

    public class SchemeArray : INotifyPropertyChanged
    {
        private event Action<SchemeType, UpdateStatus, int> event_updateCollection;
        public event Action <SchemeType, UpdateStatus, int> Event_updateCollection
        {
            add
            {
                this.event_updateCollection -= value;
                this.event_updateCollection += value;
            }
            remove => this.event_updateCollection -= value;
        }

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

        public SchemeType Type { get; private set; }

        public ListExt<int> SchemePartValues { get; set; }

        public SchemeArray(string title, SchemeType type)
        {
            this.event_updateCollection = null;
            this.Type = type;
            this.titleSchemePart = title;
            this.SchemePartValues = new ListExt<int>();
            this.PropertyChanged = null;
            this.SchemePartValues.CollectionChanged += this.UpdateTrigger;
        }
        
        private void UpdateTrigger(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateStatus status = UpdateStatus.Default;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    status = UpdateStatus.Add;
                    this.event_updateCollection?.Invoke(this.Type, status, (int)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    status = UpdateStatus.Delete;
                    this.event_updateCollection?.Invoke(this.Type, status, (int)e.OldItems[0]);
                    break;
            }
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
