using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Static;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.Models.Categories
{
    public enum SchemeType
    {
        Platform,
        Block
    }

    public delegate void UpdateSchemeArray(SchemeType type, UpdateStatus status, IdCheck value, int old_index, int new_index);

    public class SchemeArray : INotifyPropertyChanged, IDropTarget
    {
        private event UpdateSchemeArray event_updateCollection;
        public event UpdateSchemeArray Event_updateCollection
        {
            add
            {
                this.event_updateCollection -= value;
                this.event_updateCollection += value;
            }
            remove => this.event_updateCollection -= value;
        }

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

        public ListExt<IdCheck> SchemePartValues { get; set; }

        public SchemeArray(string title, SchemeType type)
        {
            this.Type = type;
            this.titleSchemePart = title;
            this.SchemePartValues = new ListExt<IdCheck>();
            this.SchemePartValues.CollectionChanged += this.UpdateTrigger;
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IdCheck check && this.SchemePartValues.Contains(check))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IdCheck check)
            {
                int oldIndex = this.SchemePartValues.IndexOf(check);
                int newIndex = dropInfo.InsertIndex;

                if (newIndex > oldIndex) newIndex -= 1;

                this.SchemePartValues.Move(oldIndex, newIndex);
            }
        }

        private void UpdateTrigger(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                /*
                case NotifyCollectionChangedAction.Add:
                    status = UpdateStatus.Add;
                    this.event_updateCollection?.Invoke(this.Type, status, (IdCheck)e.NewItems[0]);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    status = UpdateStatus.Delete;
                    this.event_updateCollection?.Invoke(this.Type, status, (IdCheck)e.OldItems[0]);
                    break;
                    */
                case NotifyCollectionChangedAction.Add:
                    ((IdCheck)e.NewItems[0]).Event_UpdateCheck += this.UpdateCheck;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((IdCheck)e.OldItems[0]).Event_UpdateCheck -= this.UpdateCheck;
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                    this.event_updateCollection?.Invoke(this.Type, UpdateStatus.Move, (IdCheck)e.NewItems[0], e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }

        private void UpdateCheck()
        {
            this.event_UpdateCheck?.Invoke();
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
