using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Interfaces;
using DanceRegUltra.ViewModels;

namespace DanceRegUltra.Models.Categories
{
    public class DanceScheme : NotifyPropertyChanged
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

        public DanceScheme()
        {
            this.Id_scheme = -1;
            this.title_scheme = "";
            

            this.SchemeLeagues = new ListExt<IdCheck>(SchemeManagerViewModel.AllLeagues);
            this.SchemeAges = new ListExt<IdCheck>(SchemeManagerViewModel.AllAges);
            this.SchemeStyles = new ListExt<IdCheck>();
            foreach (IdCheck style in SchemeManagerViewModel.AllStyles)
            {
                this.SchemeStyles.Add(new IdCheck(style.Id, style.IsChecked));
            }

            this.PlatformsCollection = new ListExt<SchemeArray>();
            this.BlocksCollection = new ListExt<SchemeArray>();

            this.AddPlatform();
            this.AddBlock();
            this.AddBlock();
            this.AddBlock();
            this.AddBlock();
        }

        public void AddPlatform(string title = "Платформа")
        {
            SchemeArray platform = new SchemeArray(title, SchemeType.Platform);
            foreach(IdCheck league in this.SchemeLeagues)
            {
                platform.SchemePartValues.Add(new IdCheck(league.Id, league.IsChecked));
            }
            platform.Event_updateCollection += this.UpdateSchemeArray;
            this.PlatformsCollection.Add(platform);
        }

        public void AddBlock(string title = "Блок")
        {
            SchemeArray block = new SchemeArray(title, SchemeType.Block);
            foreach(IdCheck age in this.SchemeAges)
            {
                block.SchemePartValues.Add(new IdCheck(age.Id, age.IsChecked));
            }
            block.Event_updateCollection += this.UpdateSchemeArray;
            this.BlocksCollection.Add(block);
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
                        break;
                }
            }
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
                this.BlocksCollection.Remove(block);
            },
                (block) => block != null && block.Type == SchemeType.Block && this.BlocksCollection.Count > 1);
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
