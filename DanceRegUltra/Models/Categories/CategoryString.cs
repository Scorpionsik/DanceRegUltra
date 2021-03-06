﻿using DanceRegUltra.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanceRegUltra.Models.Categories
{
   

    public delegate void UpdateCategoryString(int id, CategoryType type, string columnName, object value);

    public class CategoryString : INotifyPropertyChanged, IComparable<CategoryString>
    {
        private TimerCallback NameUpdate_Callback;
        private Timer NameUpdate_Timer;

        private void NameUpdateMethod(object value)
        {
            if(this.NameUpdate_Timer != null) this.NameUpdate_Timer.Dispose();
            this.event_updateCategoryString?.Invoke(this.Id, this.Type, "Name", value);
        }

        private event UpdateCategoryString event_updateCategoryString;
        
        public event UpdateCategoryString Event_UpdateCategoryString
        {
            add
            {
                this.event_updateCategoryString -= value;
                this.event_updateCategoryString += value;
            }
            remove => this.event_updateCategoryString -= value;
        }
        /*
        private bool updateFlag;
        public bool UpdateFlag
        {
            get => this.updateFlag;
            set
            {
                this.updateFlag = value;
                this.OnPropertyChanged("UpdateFlag");
            }
        }*/

        public int Id { get; private set; }

        private int position;
        public int Position
        {
            get => this.position;
            set
            {
                this.position = value;
                this.event_updateCategoryString?.Invoke(this.Id, this.Type, "Position", value);
                //this.UpdateFlagChange();
            }
        }

        private bool isHide;
        public bool IsHide
        {
            get => this.isHide;
            set
            {
                this.isHide = value;
                this.OnPropertyChanged("IsHide");
                this.event_updateCategoryString?.Invoke(this.Id, this.Type, "IsHide", value);
                //this.UpdateFlagChange();
            }
        }

        public CategoryType Type { get; private set; }

        private string name;
        public string Name
        {
            get => this.name;
            set
            {
                if (this.NameUpdate_Timer != null) this.NameUpdate_Timer.Dispose();
                this.name = value;
                this.OnPropertyChanged("Name");
                this.NameUpdate_Timer = new Timer(this.NameUpdate_Callback, this.name, 1000, 0);
                //this.UpdateFlagChange();
            }
        }

        public CategoryString(int id, CategoryType type, string name = "", int position = 0, bool isHide = false)
        {
            this.event_updateCategoryString = null;
            this.Id = id;
            this.Type = type;
            this.name = name;
            this.position = position;
            this.isHide = isHide;

            this.NameUpdate_Callback = new TimerCallback(this.NameUpdateMethod);
        }

        public int CompareTo(CategoryString obj)
        {
            int this_pos = this.Position == 0 ? this.Id : this.Position;
            int obj_pos = obj.Position == 0 ? obj.Id : obj.Position;

            return obj_pos - this_pos;
        }

        /*
        private void UpdateFlagChange(bool flag = true)
        {
            if (this.UpdateFlag != flag) this.UpdateFlag = flag;
        }*/

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
        } //---метод OnPropertyChanged

       
    }
}
