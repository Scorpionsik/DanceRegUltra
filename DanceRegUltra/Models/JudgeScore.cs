using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models.Categories;
using DanceRegUltra.Utilites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models
{
    public class JudgeScore
    {
        public int Judge_number { get; private set; }
        public ListExt<Title> Scores { get; private set; }

        public JudgeScore()
        {
            this.Judge_number = 0;
            this.Scores = new ListExt<Title>();
        }

        public JudgeScore(int number, IEnumerable<double> scores)
        {
            this.Judge_number = number;
            this.Scores = new ListExt<Title>();
            foreach(double score in scores)
            {
                this.Scores.Add(new Title(score.ToString()));
            }
        }

        public List<double> GetScores()
        {
            List<double> res = new List<double>();

            foreach(Title score in this.Scores)
            {
                res.Add(Convert.ToDouble(score.Value.Replace('.',',')));
            }

            return res;
        }
    }
}
