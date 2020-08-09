using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using DanceRegUltra.Static;
using Newtonsoft.Json;

namespace DanceRegUltra.Models
{
    public class DanceScheme : INotifyPropertyChanged
    {
        public int Id_scheme { get; private set; }

        private string title_scheme;
        public string Title_scheme
        {
            get => this.title_scheme;
            set
            {
                this.title_scheme = value;
                this.OnPropertyChanged("Title_scheme");
            }
        }

        public ListExt<int> LeagueValues { get; private set; }

        public ListExt<int> AgeValues { get; private set; }

        public ListExt<int> StyleValues { get; private set; }

        public ListExt<SchemeArray> PlatformsCollection { get; private set; }

        public ListExt<SchemeArray> BlocksCollection { get; private set; }

        public DanceScheme(int id_scheme = -1)
        {
            this.Id_scheme = id_scheme;
            this.title_scheme = "Test";
            this.LeagueValues = new ListExt<int>();
            this.AgeValues = new ListExt<int>();
            this.StyleValues = new ListExt<int>();

            this.PlatformsCollection = new ListExt<SchemeArray>();
            this.BlocksCollection = new ListExt<SchemeArray>();
            this.PropertyChanged = null;
            this.AddPlatform();
            this.AddBlock();
            this.AddBlock();
            this.AddBlock();
            this.AddBlock();
        }

        public void AddPlatform(string title = "Платформа")
        {
            SchemeArray platform = new SchemeArray(title, SchemeType.Platform);
            platform.Event_updateCollection += this.UpdateValues;
            this.PlatformsCollection.Add(platform);
        }

        public void AddBlock(string title = "Блок")
        {
            SchemeArray block = new SchemeArray(title, SchemeType.Block);
            block.Event_updateCollection += this.UpdateValues;
            this.BlocksCollection.Add(block);
        }

        private void UpdateValues(SchemeType type, UpdateStatus status, int value)
        {
            switch (status)
            {
                case UpdateStatus.Add:
                    switch (type)
                    {
                        case SchemeType.Platform:
                            if (!this.LeagueValues.Contains(value)) this.LeagueValues.Insert(this.CheckCategoryPosition(DanceRegCollections.Leagues.Value, this.LeagueValues, value) , value);
                            break;
                        case SchemeType.Block:
                            if (!this.AgeValues.Contains(value)) this.AgeValues.Insert(this.CheckCategoryPosition(DanceRegCollections.Ages.Value, this.AgeValues, value), value);
                            break;
                    }
                    break;
                case UpdateStatus.Delete:
                    switch (type)
                    {
                        case SchemeType.Platform:
                            if (this.LeagueValues.Contains(value)) this.LeagueValues.Remove(value);
                            break;
                        case SchemeType.Block:
                            if (this.AgeValues.Contains(value)) this.AgeValues.Remove(value);
                            break;
                    }
                    break;
            }
        }

        private int CheckCategoryPosition(IEnumerable<CategoryString> categories, IEnumerable<int> scheme_categories, int value)
        {
            int scheme_category_index = 0;
            if (scheme_categories.Count() == 0) return 0;
            else
            {
                foreach (CategoryString category in categories)
                {
                    if (category.Id == value) return scheme_category_index;
                    else if (category.Id == scheme_categories.ElementAt(scheme_category_index)) scheme_category_index++;
                }
            }
            return -1;
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
        } //---метод OnPropertyChanged
    }
}
