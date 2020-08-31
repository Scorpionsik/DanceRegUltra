using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Utilites.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class SelectSchemeTypeViewModel : ViewModel
    {
        private event Action<IdTitle> event_SetTitle;
        public event Action<IdTitle> Event_SetTitle
        {
            add
            {
                this.event_SetTitle -= value;
                this.event_SetTitle += value;
            }
            remove => this.event_SetTitle -= value;
        }

        public List<IdTitle> Values { get; private set; }

        private IdTitle select_value;
        public IdTitle Select_value
        {
            get => this.select_value;
            set
            {
                this.select_value = value;
                this.OnPropertyChanged("Select_value");
                
            }
        }

        public SelectSchemeTypeViewModel(int category_id, SchemeType type, IEnumerable<IdTitle> values, IdTitle select_title)
        {
            string category_name = CategoryNameByIdConvert.Convert(category_id, type == SchemeType.Platform ? CategoryType.League : CategoryType.Age);

            this.Title = "Выберите " + (type == SchemeType.Platform ? "платформу" : "блок") + " для " + (type == SchemeType.Platform ? "лиги" : "возрастной категории") + " '" + category_name + "'";

            this.Values = new List<IdTitle>(values);
            this.Select_value = select_title;
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.event_SetTitle?.Invoke(this.Select_value);
                base.Command_save?.Execute();
            },
                (obj) => this.Select_value != null);
        }
    }
}
