using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class PlatformsManagerViewModel : ViewModel, IDropTarget
    {
        private DanceEvent EventInWork;
        public ListExt<JsonSchemeArray> Platforms { get; private set; }

        private JsonSchemeArray select_platform;
        public JsonSchemeArray Select_platform
        {
            get => this.select_platform;
            set
            {
                this.select_platform = value;
                this.OnPropertyChanged("Select_platform");
            }
        }



        public PlatformsManagerViewModel(int event_id)
        {
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.Platforms = new ListExt<JsonSchemeArray>();

            this.Title = "[" + this.EventInWork.Title + "] Редактор площадок - " + App.AppTitle;

            foreach (JsonSchemeArray platform in this.EventInWork.SchemeEvent.Platforms)
            {
                JsonSchemeArray newPlatform = new JsonSchemeArray();
                newPlatform.IdArray = this.Platforms.Count + 1;
                newPlatform.Title = platform.Title;
                foreach(IdCheck value in platform.Values)
                {
                    newPlatform.Values.Add(new IdCheck(value.Id, value.IsChecked));
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

        public RelayCommand<JsonSchemeArray> Command_RemovePlatform
        {
            get => new RelayCommand<JsonSchemeArray>(platform =>
            {
                int index = this.Platforms.IndexOf(platform) > 0 ? 0 : 1;
                for(int i = 0; i < platform.Values.Count; i++)
                {
                    if (platform.Values[i].IsChecked) this.Platforms[index].Values[i].IsChecked = true;
                }
                this.Platforms.Remove(platform);
                index = 1;
                foreach(JsonSchemeArray p in this.Platforms)
                {
                    p.IdArray = index;
                    p.Title = "Платформа " + index;
                    index++;
                }
                this.OnPropertyChanged("Platforms");
            },
                (platform) => platform != null && this.Platforms != null && this.Platforms.Count > 1);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is IdCheck league)
            {
                int index_insert = dropInfo.InsertIndex - 1;
                if (index_insert < 0) index_insert = 0;
                for(int i = 0; i < this.Platforms.Count; i++)
                {
                    foreach(IdCheck value in this.Platforms[i].Values)
                    {
                        if (league.Id == value.Id) value.IsChecked = i == index_insert ? true : false;
                    }
                }
            }
        }

        private async void SaveMethod()
        {
            this.EventInWork.SchemeEvent.Platforms.RemoveAll(new Predicate<JsonSchemeArray>(obj => true));
            this.EventInWork.SchemeEvent.Platforms.AddRange(this.Platforms);
            await DanceRegDatabase.ExecuteNonQueryAsync("update events set Json_scheme='" + JsonScheme.Serialize(this.EventInWork.SchemeEvent) + "' where Id_event=" + this.EventInWork.IdEvent);
            bool next = false;
            foreach(DanceNode node in this.EventInWork.Nodes)
            {
                foreach(JsonSchemeArray platform in this.Platforms)
                {
                    foreach(IdCheck value in platform.Values)
                    {
                        if(value.IsChecked && value.Id == node.LeagueId && platform.IdArray != node.Platform.Id)
                        {
                            for (int i = 0; i < this.EventInWork.Leagues[node.LeagueId].Count; i++)
                            {
                                if(this.EventInWork.Leagues[node.LeagueId][i].Id == node.Platform.Id)
                                {
                                    this.EventInWork.Leagues[node.LeagueId].RemoveAt(i);
                                    this.EventInWork.Leagues[node.LeagueId].Add(new IdTitle(platform.IdArray, platform.Title));
                                    break;
                                }
                            }
                            node.SetPlatform(new IdTitle(platform.IdArray, platform.Title));
                            //await DanceRegDatabase.ExecuteNonQueryAsync("update event_nodes set Id_platform=" + platform.IdArray + " where Id_event=" + this.EventInWork.IdEvent + " and Id_node=" + node.NodeId);
                            next = true;
                            break;
                        }
                    }
                    if (next)
                    {
                        next = false;
                        break;
                    }
                }
            }
            base.Command_save?.Execute();
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveMethod();
            });
        }

    }
}
