using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models
{
    public enum CategoryType
    {
        League,
        Age,
        Style
    }

    public struct CategoryString : INotifyPropertyChanged, IComparable
    {
        private event Action<int, CategoryType> event_updateCategoryString;
        public event Action<int, CategoryType> Event_UpdateCategoryString
        {
            add
            {
                this.event_updateCategoryString -= value;
                this.event_updateCategoryString += value;
            }
            remove => this.event_updateCategoryString -= value;
        }

        public int Id { get; private set; }

        private int position;
        public int Position
        {
            get => this.position;
            set
            {
                this.position = value;
                //this.event_updateCategoryString?.Invoke(this.Id, this.Type);
            }
        }

        public CategoryType Type { get; private set; }

        private string name;
        public string Name
        {
            get => this.name;
            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
                this.event_updateCategoryString?.Invoke(this.Id, this.Type);
            }
        }

        public CategoryString(int id, CategoryType type, string name = "", int position = 0)
        {
            this.event_updateCategoryString = null;
            this.Id = id;
            this.Type = type;
            this.name = name;
            this.position = position;
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

        public int CompareTo(object obj)
        {
            if (obj is CategoryString category)
            {
                int this_pos = this.Position == 0 ? this.Id : this.Position;
                int obj_pos = category.Position == 0 ? category.Id : category.Position;

                return obj_pos - this_pos;
            }
            else throw new Exception("Not a CategoryString");
        }
        //---метод OnPropertyChanged
    }
}
