using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Enums;
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
    public class JudgeManagerViewModel : ViewModel
    {
        private DanceEvent EventInWork;

        public ListExt<JsonSchemeArray> Blocks { get; private set; }

        private int commonJudgeCount;
        public int CommonJudgeCount
        {
            get => this.commonJudgeCount;
            set
            {
                this.commonJudgeCount = value;
                this.OnPropertyChanged("CommonJudgeCount");
            }
        }
         
        public List<JudgeType> ScoreTypes { get; private set; }

        private JudgeType commonScoreType;
        public JudgeType CommonScoreType
        {
            get => this.commonScoreType;
            set
            {
                this.commonScoreType = value;
                this.OnPropertyChanged("CommonScoreType");
            }
        }

        public JudgeManagerViewModel(int event_id)
        {
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.Title = "[" + this.EventInWork.Title + "] Настройки судейства - " + App.AppTitle;

            this.Blocks = new ListExt<JsonSchemeArray>();
            this.ScoreTypes = JsonSchemeArray.ScoreTypes;

            foreach (JsonSchemeArray block in this.EventInWork.SchemeEvent.Blocks)
            {
                JsonSchemeArray newBlock = new JsonSchemeArray();
                newBlock.IdArray = block.IdArray;
                newBlock.Title = block.Title;
                newBlock.ScoreType = block.ScoreType;
                newBlock.JudgeCount = block.JudgeCount;
                this.Blocks.Add(newBlock);
            }

            this.CommonJudgeCount = 4;
            this.CommonScoreType = JudgeType.ThreeD;
        }

        private async void SaveMethod()
        {
            foreach(JsonSchemeArray block in this.Blocks)
            {
                JsonSchemeArray event_block = this.EventInWork.SchemeEvent.GetSchemeArrayById(block.IdArray, SchemeType.Block);

                event_block.JudgeCount = block.JudgeCount;
                event_block.ScoreType = block.ScoreType;
                foreach(DanceNomination nomination in this.EventInWork.Nominations)
                {
                    if (nomination.Block_info.Id != event_block.IdArray) continue;
                    nomination.SetNewType(block.ScoreType);
                    nomination.SetJudgeCount(block.JudgeCount);
                    await DanceRegDatabase.ExecuteNonQueryAsync("update nominations set Json_judge_ignore='" + nomination.GetJsonJudgeIgnore() + "' where Id_event=" + this.EventInWork.IdEvent + " and Id_block=" + nomination.Block_info.Id + " and Id_league=" + nomination.League_id + " and Id_age=" + nomination.Age_id + " and Id_style=" + nomination.Style_id);
                }
            }

            await DanceRegDatabase.ExecuteNonQueryAsync("update events set Json_scheme='" + JsonScheme.Serialize(this.EventInWork.SchemeEvent) + "' where Id_event=" + this.EventInWork.IdEvent);

            base.Command_save?.Execute();
        }

        public RelayCommand Command_SetCommon
        {
            get => new RelayCommand(obj =>
            {
                foreach(JsonSchemeArray block in this.Blocks)
                {
                    block.JudgeCount = this.CommonJudgeCount;
                    block.ScoreType = this.CommonScoreType;
                }
            });
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
