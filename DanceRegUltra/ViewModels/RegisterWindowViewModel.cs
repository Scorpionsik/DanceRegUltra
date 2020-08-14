using CoreWPF.MVVM;
using CoreWPF.Utilites;
using CoreWPF.Windows.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels
{
    public class RegisterWindowViewModel : ViewModel
    {
        private event Action<DanceEvent> event_SetReturnEvent;
        public event Action<DanceEvent> Event_SetReturnEvent
        {
            add
            {
                this.event_SetReturnEvent -= value;
                this.event_SetReturnEvent += value;
            }
            remove => this.event_SetReturnEvent -= value;
        }

        private string titleEvent;
        public string TitleEvent
        {
            get => this.titleEvent;
            set
            {
                this.titleEvent = value;
                this.OnPropertyChanged("TitleEvent");
            }
        }

        private DateTime startDateEvent;
        public DateTime StartDateEvent
        {
            get => this.startDateEvent;
            set
            {
                this.startDateEvent = value;
                this.OnPropertyChanged("StartDateEvent");
            }
        }

        private DanceScheme schemeEvent;
        public DanceScheme SchemeEvent
        {
            get => this.schemeEvent;
            private set
            {
                this.schemeEvent = value;
                this.OnPropertyChanged("SchemeEvent");
            }
        }

        public RegisterWindowViewModel() : base()
        {
            this.StartDateEvent = UnixTime.CurrentDateTimeOffset(App.Locality).DateTime;
        }

        public override WindowClose CloseMethod()
        {
            return base.CloseMethod();
        }

        private async void CreateReturnEvent()
        {
            await Task.Run(() =>
            {
                JsonScheme tmp_scheme = new JsonScheme(this.SchemeEvent);

                this.event_SetReturnEvent?.Invoke(new DanceEvent(-1, this.TitleEvent, UnixTime.ToUnixTimestamp(new DateTimeOffset(this.StartDateEvent, App.Locality)), -1, JsonScheme.Serialize(tmp_scheme)));
            });
            base.Command_save?.Execute();
        }

        public RelayCommand Command_SetScheme
        {
            get => new RelayCommand(obj =>
            {
                SchemeManagerView window = new SchemeManagerView(this.SchemeEvent == null ? 0 : this.SchemeEvent.Id_scheme);
                if ((bool)window.ShowDialog())
                {
                    this.SchemeEvent = window.Return_scheme;
                }
            });
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.CreateReturnEvent();
            });
        }
    }
}
