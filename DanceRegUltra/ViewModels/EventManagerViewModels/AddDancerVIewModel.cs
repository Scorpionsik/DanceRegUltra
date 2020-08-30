using CoreWPF.MVVM;
using DanceRegUltra.Enums;
using DanceRegUltra.Models;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Static;
using DanceRegUltra.Utilites;
using DanceRegUltra.Views.EventManagerViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class AddDancerViewModel : ViewModel
    {
        public DanceEvent EventInWork { get; private set; }
        
        internal FindDancer FindList { get; private set; }

        private MemberDancer dancerInWork;
        public MemberDancer DancerInWork
        {
            get => this.dancerInWork;
            private set
            {
                this.dancerInWork = value;
                this.OnPropertyChanged("DancerInWork");
            }
        }

        private IdTitle select_platform;
        public IdTitle Select_platform
        {
            get => this.select_platform;
            private set
            {
                this.select_platform = value;
                this.OnPropertyChanged("Select_platform");
            }
        }

        private KeyValuePair<int, List<IdTitle>> select_league;
        public KeyValuePair<int, List<IdTitle>> Select_league
        {
            get => this.select_league;
            set
            {
                this.select_league = value;
                this.OnPropertyChanged("Select_league");
                this.SetSchemeType(SchemeType.Platform, value.Value);
            }
        }

        private IdTitle select_block;
        public IdTitle Select_block
        {
            get => this.select_block;
            private set
            {
                this.select_block = value;
                this.OnPropertyChanged("Select_block");
            }
        }

        private KeyValuePair<int, List<IdTitle>> select_age;
        public KeyValuePair<int, List<IdTitle>> Select_age
        {
            get => this.select_age;
            set
            {
                this.select_age = value;
                this.OnPropertyChanged("Select_age");
                this.SetSchemeType(SchemeType.Block, value.Value);
            }
        }

        public List<IdCheck> Styles { get; private set; }

        public AddDancerViewModel(int event_id)
        {
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.DancerInWork = new MemberDancer(event_id, -1, "", "");
            this.FindList = new FindDancer(1000);
            this.Styles = new List<IdCheck>();
            foreach(int style in this.EventInWork.Styles)
            {
                this.Styles.Add(new IdCheck(style));
            }
        }

        private void SetSchemeType(SchemeType type, IEnumerable<IdTitle> values)
        {
            switch (type)
            {
                case SchemeType.Platform:
                    if (values.Count() == 1) this.Select_platform = values.ElementAt(0);
                    else
                    {
                        SelectSchemeTypeView window = new SelectSchemeTypeView(this.Select_league.Key, SchemeType.Platform, this.Select_league.Value);
                        if ((bool)window.ShowDialog())
                        {
                            this.Select_platform = window.Select_value;
                        }

                    }
                    break;
                case SchemeType.Block:
                    if (values.Count() == 1) this.Select_block = values.ElementAt(0);
                    else
                    {
                        SelectSchemeTypeView window = new SelectSchemeTypeView(this.Select_age.Key, SchemeType.Block, this.Select_age.Value);
                        if ((bool)window.ShowDialog())
                        {
                            this.Select_block = window.Select_value;
                        }
                    }
                    break;
            }
        }
  
    }
}
