using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using DanceRegUltra.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.ViewModels.EventManagerViewModels
{
    public class SetScoresForNodeViewModel : ViewModel
    {
        private DanceEvent EventInWork;

        public DanceNomination NominationInWork { get; private set; }
        public DanceNode NodeInWork { get; private set; }

        public ListExt<JudgeScore> Scores { get; private set; }

        public SetScoresForNodeViewModel(int event_id, DanceNode node)
        {
            this.EventInWork = DanceRegCollections.GetEventById(event_id);
            this.NodeInWork = node;
            this.NominationInWork = this.EventInWork.GetNominationByNode(node);

            this.Scores = new ListExt<JudgeScore>();

            int judge_count = this.NominationInWork.JudgeCount;
            int score_count = this.NominationInWork.Type == Enums.JudgeType.ThreeD ? 3 : 4;
            for(int j = 0; j < judge_count; j++)
            {
                List<int> judge_scores = node.Scores.Count > j ? node.Scores[j] : new List<int>() { 0, 0, 0, 0 };
                ListExt<int> tmp_scores = new ListExt<int>();
                for (int i = 0; i < score_count; i++)
                {
                    tmp_scores.Add(judge_scores.Count >= score_count ? judge_scores[i] : 0);
                }
                this.Scores.Add(new JudgeScore(j+1, tmp_scores));
            }
        }

        private async void SaveMethod()
        {
            List<List<int>> add_scores = new List<List<int>>();
            foreach(JudgeScore edit_score in this.Scores)
            {
                add_scores.Add(edit_score.GetScores());
            }
            this.NodeInWork.SetScores(add_scores);
            await DanceRegDatabase.ExecuteNonQueryAsync("update event_nodes set Json_scores = '"+ this.NodeInWork.GetScores() +"' where Id_event=" + this.EventInWork.IdEvent + " and Id_node=" + this.NodeInWork.NodeId + ";");
            await this.NominationInWork.CheckNodeScores();

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
