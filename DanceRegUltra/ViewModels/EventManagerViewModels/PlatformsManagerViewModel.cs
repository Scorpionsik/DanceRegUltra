using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class PlatformsManagerViewModel : ViewModel
    {
        private DanceEvent EventInWork;
        public ListExt<JsonSchemeArray> Platforms { get; private set; }


        public PlatformsManagerViewModel(int event_id)
        {
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.Platforms = new ListExt<JsonSchemeArray>();

            foreach (JsonSchemeArray platform in this.EventInWork.SchemeEvent.Platforms)
            {
                JsonSchemeArray newPlatform = new JsonSchemeArray();
                newPlatform.IdArray = platform.IdArray;
                newPlatform.Title = platform.Title;
                foreach(IdCheck value in platform.Values)
                {
                    newPlatform.Values.Add(new IdCheck(value.Id));
                }
                this.Platforms.Add(newPlatform);
            }
        }

        public RelayCommand Command_AddPlatform
        {
            get => new RelayCommand(obj =>
            {
                JsonSchemeArray tmp_add = new JsonSchemeArray();
                tmp_add.IdArray = this.Platforms.Count + 1;
                tmp_add.Title = "Платформа " + tmp_add.IdArray;
                foreach(IdCheck value in this.Platforms.First.Values)
                {
                    tmp_add.Values.Add(new IdCheck(value.Id));
                }
                this.Platforms.Add(tmp_add);
            });
        }
    }
}
