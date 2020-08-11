using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Interfaces;
using DanceRegUltra.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace DanceRegUltra.Models.Categories
{
    public class DanceScheme : NotifyPropertyChanged 
    {
        private event Action<bool> event_UpdateDanceScheme;
        public event Action<bool> Event_UpdateDanceScheme
        {
            add
            {
                this.event_UpdateDanceScheme -= value;
                this.event_UpdateDanceScheme += value;
            }
            remove => this.event_UpdateDanceScheme -= value;
        }

        private bool updateFlag;
        public bool UpdateFlag
        {
            get => this.updateFlag;
            set
            {
                this.updateFlag = value;
                this.OnPropertyChanged("UpdateFlag");
            }
        }

        public int Id_scheme { get; private set; }

        private string title_scheme;
        public string Title_scheme
        {
            get => this.title_scheme;
            set
            {
                this.title_scheme = value;
                this.OnPropertyChanged("Title_scheme");
                this.UpdateFlagChange(true, true);
            }
        }

        public ListExt<IdCheck> SchemeLeagues { get; private set; }
        public ListExt<IdCheck> SchemeAges { get; private set; }

        public ListExt<SchemeArray> PlatformsCollection { get; private set; }
        public ListExt<SchemeArray> BlocksCollection { get; private set; }

        private SchemeArray select_platform;
        public SchemeArray Select_platform
        {
            get => this.select_platform;
            set
            {
                this.select_platform = value;
                this.OnPropertyChanged("Select_platform");
            }
        }

        private SchemeArray select_block;
        public SchemeArray Select_block
        {
            get => this.select_block;
            set
            {
                this.select_block = value;
                this.OnPropertyChanged("Select_block");
            }
        }

        public ListExt<IdCheck> SchemeStyles { get; private set; }

        public DanceScheme(string title = "")
        {
            this.UpdateFlag = true;
            this.Id_scheme = -1;
            this.title_scheme = title;
            

            this.SchemeLeagues = new ListExt<IdCheck>(SchemeManagerViewModel.AllLeagues);
            this.SchemeAges = new ListExt<IdCheck>(SchemeManagerViewModel.AllAges);
            this.SchemeStyles = new ListExt<IdCheck>();
            this.SchemeStyles.CollectionChanged += this.UpdateStyleTrigger;

            foreach (IdCheck style in SchemeManagerViewModel.AllStyles)
            {
                IdCheck add_style = new IdCheck(style.Id, style.IsChecked);
                add_style.Event_UpdateCheck += this.UpdateCheck;
                this.SchemeStyles.Add(add_style);
            }

            this.PlatformsCollection = new ListExt<SchemeArray>();
            this.BlocksCollection = new ListExt<SchemeArray>();

            this.AddPlatform("Платформа 1");
            this.AddBlock("Блок 1");
            this.AddBlock("Блок 2");
            this.AddBlock("Блок 3");
            this.AddBlock("Блок 4");
        }

        public DanceScheme(int id, string title, JsonScheme scheme, bool isNewScheme = false)
        {
            
            this.Id_scheme = id;
            this.title_scheme = title;

            this.SchemeLeagues = new ListExt<IdCheck>();
            this.SchemeAges = new ListExt<IdCheck>();
            this.SchemeStyles = new ListExt<IdCheck>();
            this.SchemeStyles.CollectionChanged += this.UpdateStyleTrigger;

            this.PlatformsCollection = new ListExt<SchemeArray>();
            this.BlocksCollection = new ListExt<SchemeArray>();

            bool isNew = false;
            foreach(JsonSchemeArray platform in scheme.Platforms)
            {
                if (!isNew)
                {
                    foreach(IdCheck value in platform.Values)
                    {
                        this.SchemeLeagues.Add(new IdCheck(value.Id));
                    }
                    isNew = true;
                }
                this.AddPlatform(platform.Title, platform.Values);
            }

            isNew = false;
            foreach(JsonSchemeArray block in scheme.Blocks)
            {
                if (!isNew)
                {
                    foreach(IdCheck value in block.Values)
                    {
                        this.SchemeAges.Add(new IdCheck(value.Id));
                    }
                    isNew = true;
                }
                this.AddBlock(block.Title, block.Values);
            }

            foreach(IdCheck style in scheme.Styles)
            {
                IdCheck add_style = new IdCheck(style.Id, style.IsChecked);
                add_style.Event_UpdateCheck += this.UpdateCheck;
                this.SchemeStyles.Add(add_style);
            }
            this.UpdateFlag = isNewScheme == false ? false : true;
        }

        public void AddPlatform(string title = "Платформа", IEnumerable<IdCheck> collection = null)
        {
            SchemeArray platform = new SchemeArray(title, SchemeType.Platform);
            if (collection == null) collection = this.SchemeLeagues;
            foreach (IdCheck league in collection)
            {
                platform.SchemePartValues.Add(new IdCheck(league.Id, league.IsChecked));
            }
            platform.Event_updateCollection += this.UpdateSchemeArray;
            platform.Event_UpdateCheck += this.UpdateCheck;
            this.PlatformsCollection.Add(platform);
            this.UpdateFlagChange();
        }

        public void AddBlock(string title = "Блок", IEnumerable<IdCheck> collection = null)
        {
            SchemeArray block = new SchemeArray(title, SchemeType.Block);
            if (collection == null) collection = this.SchemeAges;
            foreach (IdCheck age in collection)
            {
                block.SchemePartValues.Add(new IdCheck(age.Id, age.IsChecked));
            }
            block.Event_updateCollection += this.UpdateSchemeArray;
            block.Event_UpdateCheck += this.UpdateCheck;
            this.BlocksCollection.Add(block);
            this.UpdateFlagChange();
        }

        private void UpdateCheck()
        {
            this.UpdateFlagChange();
        }

        private void UpdateStyleTrigger(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateFlagChange();
        }

        private void UpdateSchemeArray(SchemeType type, UpdateStatus status, IdCheck value, int old_index, int new_index)
        {

            if (old_index != new_index)
            {
                switch (status)
                {
                    case UpdateStatus.Move:
                        switch (type)
                        {
                            case SchemeType.Platform:
                                this.SchemeLeagues.Move(old_index, new_index);
                                foreach (SchemeArray platform in this.PlatformsCollection)
                                {
                                    if (platform.SchemePartValues[new_index].Id != value.Id)
                                    {
                                        platform.SchemePartValues.Move(old_index, new_index);
                                    }
                                }
                                break;
                            case SchemeType.Block:
                                this.SchemeAges.Move(old_index, new_index);
                                foreach (SchemeArray block in this.BlocksCollection)
                                {
                                    if (block.SchemePartValues[new_index].Id != value.Id)
                                    {
                                        block.SchemePartValues.Move(old_index, new_index);
                                    }
                                }
                                break;
                        }
                        this.UpdateFlagChange();
                        break;
                }
            }
        }

        private void UpdateFlagChange(bool flag = true, bool isUpdateTitle = false)
        {
            if(this.UpdateFlag != flag) this.UpdateFlag = flag;
            this.event_UpdateDanceScheme?.Invoke(isUpdateTitle);
        }

        public RelayCommand Command_AddPlatform
        {
            get => new RelayCommand(obj =>
            {
                this.AddPlatform();
            });
        }

        public RelayCommand<SchemeArray> Command_DeletePlatform
        {
            get => new RelayCommand<SchemeArray>(platform =>
            {
                this.UpdateFlagChange();
                platform.Event_updateCollection -= this.UpdateSchemeArray;
                platform.Event_UpdateCheck -= this.UpdateCheck;
                this.PlatformsCollection.Remove(platform);
            },
                (platform) => platform != null && platform.Type == SchemeType.Platform && this.PlatformsCollection.Count > 1);
        }

        public RelayCommand Command_AddBlock
        {
            get => new RelayCommand(obj =>
            {
                this.AddBlock();
            });
        }

        public RelayCommand<SchemeArray> Command_DeleteBlock
        {
            get => new RelayCommand<SchemeArray>(block =>
            {
                this.UpdateFlagChange();
                block.Event_updateCollection -= this.UpdateSchemeArray;
                block.Event_UpdateCheck -= this.UpdateCheck;
                this.BlocksCollection.Remove(block);
            },
                (block) => block != null && block.Type == SchemeType.Block && this.BlocksCollection.Count > 1);
        }

        public RelayCommand Command_CheckAllStyles
        {
            get => new RelayCommand(obj =>
            {
                foreach (IdCheck value in this.SchemeStyles)
                {
                    value.IsChecked = true;
                }
            });
        }

        public RelayCommand Command_UncheckAllStyles
        {
            get => new RelayCommand(obj =>
            {
                foreach (IdCheck value in this.SchemeStyles)
                {
                    value.IsChecked = false;
                }
            });
        }
        #region для старой логики перемещения категорий внутри схемы
        /*
        
        private void UpdateValues(SchemeType type, UpdateStatus status, IdCheck value)
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

        private int CheckCategoryPosition(IEnumerable<CategoryString> categories, IEnumerable<IdCheck> scheme_categories, IdCheck value)
        {
            int scheme_category_index = 0;
            if (scheme_categories.Count() == 0) return 0;
            else
            {
                foreach (CategoryString category in categories)
                {
                    if (category.Id == value.Id) return scheme_category_index;
                    else if (category.Id == scheme_categories.ElementAt(scheme_category_index).Id) scheme_category_index++;
                }
            }
            return -1;
        }
        
         */
        #endregion

    }
}
