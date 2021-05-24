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
    public class NominationJudgesViewModel : ViewModel
    {
        private DanceEvent EventInWork;

        public ListExt<IdCheck> AllJudgeIgnore { get; private set; }

        private List<DanceNomination> Update_nominations;

        public Dictionary<DanceNomination, ListExt<IdCheck>> SelectJudgeIgnore { get; private set; }
        public Dictionary<DanceNomination, bool> SelectSeparate { get; private set; }

        public ListExt<IdCheck> ShowJudgeIgnore
        {
            get
            {
                if (this.Select_nomination == null) return null;
                else return this.SelectJudgeIgnore[this.Select_nomination];
            }
        }

        public bool ShowSeparate
        {
            get => this.Select_nomination == null ? false : this.SelectSeparate[this.Select_nomination];
            set
            {
                if(this.Select_nomination != null)
                {
                    this.SelectSeparate[this.Select_nomination] = value;
                    this.CheckUpdate();
                    this.OnPropertyChanged("ShowSeparate");
                }
            }
        }

        private bool allSeparate;
        public bool AllSeparate
        {
            get => this.allSeparate;
            set
            {
                this.allSeparate = value;
                this.OnPropertyChanged("AllSeparate");
            }
        }

        private bool Lock;

        public ListExt<DanceNomination> Nominations { get => this.EventInWork.Nominations; }

        private DanceNomination select_nomination;
        public DanceNomination Select_nomination
        {
            get => this.select_nomination;
            set
            {
                this.select_nomination = value;
                this.OnPropertyChanged("Select_nomination");
                this.OnPropertyChanged("ShowJudgeIgnore");
                this.OnPropertyChanged("ShowSeparate");
            }
        }

        public NominationJudgesViewModel(int event_id, DanceNomination select_nomination = null)
        {
            this.Lock = false;
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.Title = "[" + this.EventInWork.Title + "] Настройка номинаций - " + App.AppTitle;

            this.AllSeparate = true;
            int max_judge = 0;
            foreach(JsonSchemeArray block in this.EventInWork.SchemeEvent.Blocks)
            {
                if (block.JudgeCount > max_judge) max_judge = block.JudgeCount;
            }

            this.AllJudgeIgnore = new ListExt<IdCheck>();

            for(int i = 0; i < max_judge; i++)
            {
                this.AllJudgeIgnore.Add(new IdCheck(i + 1, false));
            }

            this.Update_nominations = new List<DanceNomination>();
            this.SelectJudgeIgnore = new Dictionary<DanceNomination, ListExt<IdCheck>>();
            this.SelectSeparate = new Dictionary<DanceNomination, bool>();

            foreach(DanceNomination nomination in this.EventInWork.Nominations)
            {
                ListExt<IdCheck> tmp_ignore = new ListExt<IdCheck>();
                foreach(IdCheck ignore in nomination.JudgeIgnore)
                {
                    IdCheck tmp_add = new IdCheck(ignore.Id, ignore.IsChecked);
                    tmp_add.Event_UpdateCheck += this.CheckUpdate;
                    tmp_ignore.Add(tmp_add);
                }
                this.SelectJudgeIgnore.Add(nomination, tmp_ignore);
                this.SelectSeparate.Add(nomination, nomination.Separate_dancer_group);
            }

            this.Select_nomination = select_nomination;
        }

        private void CheckUpdate()
        {
            if (!this.Lock && this.Select_nomination != null && !this.Update_nominations.Contains(this.Select_nomination)) this.Update_nominations.Add(this.Select_nomination);
        }

        private async void SaveChangesMethod()
        {
            foreach (DanceNomination nomination in this.Update_nominations)
            {
                for (int i = 0; i < this.SelectJudgeIgnore[nomination].Count; i++)
                {
                    nomination.JudgeIgnore[i].IsChecked = this.SelectJudgeIgnore[nomination][i].IsChecked;
                }
                nomination.SetSeparate(this.SelectSeparate[nomination]);
                await DanceRegDatabase.ExecuteNonQueryAsync("update nominations set Json_judge_ignore='" + nomination.GetJsonJudgeIgnore() + "', Separate_dancer_group=" + nomination.Separate_dancer_group + " where Id_event=" + this.EventInWork.IdEvent + " and Id_block=" + nomination.Block_info.Id + " and Id_league=" + nomination.League_id + " and Id_age=" + nomination.Age_id + " and Id_style=" + nomination.Style_id);
                await nomination.CheckNodeScores();
            }
            base.Command_save?.Execute();
        }

        public RelayCommand Command_SetAll
        {
            get => new RelayCommand(obj =>
            {
                this.Lock = true;
                foreach(DanceNomination nomination in this.EventInWork.Nominations)
                {
                    for(int i = 0; i < this.SelectJudgeIgnore[nomination].Count; i++)
                    {
                        this.SelectJudgeIgnore[nomination][i].IsChecked = this.AllJudgeIgnore[i].IsChecked;
                    }
                    this.SelectSeparate[nomination] = this.AllSeparate;
                }
                this.Lock = false;
                this.Update_nominations.AddRange(this.EventInWork.Nominations);
                this.OnPropertyChanged("ShowSeparate");
            });
        }

        public override RelayCommand Command_save
        {
            get => new RelayCommand(obj =>
            {
                this.SaveChangesMethod();
            });
        }
    }
}
