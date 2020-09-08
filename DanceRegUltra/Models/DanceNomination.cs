using CoreWPF.Utilites;
using DanceRegUltra.Models.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanceRegUltra.Models
{
    public class DanceNomination
    {
        public int Event_id { get; private set; }
        public int League_id { get; private set; }
        public int Age_id { get; private set; }
        public IdTitle Block_info { get; private set; }
        public int Style_id { get; private set; }
        public ListExt<DanceNode> Nominants { get; private set; }

        public DanceNomination(int event_id, int league, int age, IdTitle block, int style)
        {
            this.Event_id = event_id;
            this.League_id = league;
            this.Age_id = age;
            this.Block_info = new IdTitle(block.Id, block.Title);
            this.Style_id = style;
            this.Nominants = new ListExt<DanceNode>();
        }

        public void AddNominant(DanceNode nominant)
        {
            if (nominant.LeagueId == this.League_id &&
                nominant.AgeId == this.Age_id &&
                nominant.Block.Id == this.Block_info.Id &&
                nominant.StyleId == this.Style_id) this.Nominants.Add(nominant);
        }

        public void AddNominantRange(IEnumerable<DanceNode> collection)
        {
            foreach(DanceNode node in collection)
            {
                this.AddNominant(node);
            }
        }

        public DanceNode ToDanceNode()
        {
            return new DanceNode(this.Event_id, -1, null, false, null, this.League_id, this.Block_info, this.Age_id, this.Style_id);
        }
    }
}
