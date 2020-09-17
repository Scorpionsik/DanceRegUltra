using CoreWPF.Utilites;
using DanceRegUltra.Enums;
using DanceRegUltra.Models.Categories;
using GongSolutions.Wpf.DragDrop;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DanceRegUltra.Models
{
    public class DanceNomination : IDropTarget
    {
        private event Action<DanceNode> event_UpdateNominant;
        public event Action<DanceNode> Event_UpdateNominant
        {
            add
            {
                this.event_UpdateNominant -= value;
                this.event_UpdateNominant += value;
            }
            remove => this.event_UpdateNominant -= value;
        }
        public bool IsShowInList { get; private set; }
        public int Event_id { get; private set; }
        public int League_id { get; private set; }
        public int Age_id { get; private set; }
        public IdTitle Block_info { get; private set; }
        public int Style_id { get; private set; }
        public ListExt<DanceNode> Nominants { get; private set; }

        public ListExt<IdCheck> JudgeIgnore { get; private set; }

        public DanceNomination(int event_id, IdTitle block, JudgeType type, int league, int age, int style, bool isShow, string jsonJudge = "")
        {
            this.Event_id = event_id;
            this.League_id = league;
            this.Age_id = age;
            this.Block_info = new IdTitle(block.Id, block.Title);
            this.Style_id = style;
            this.IsShowInList = isShow;
            this.Nominants = new ListExt<DanceNode>();
            this.JudgeIgnore = new ListExt<IdCheck>();
            List<bool> tmp_judge = jsonJudge == "" ? new List<bool>() { false, false, false, false } : JsonConvert.DeserializeObject<List<bool>>(jsonJudge);
            for(int i = 0; i < 4; i++)
            {
                if (i == 3 && type == JudgeType.ThreeD) continue;
                this.JudgeIgnore.Add(new IdCheck(i + 1, tmp_judge[i]));
            }
        }
        public void AddNominant(DanceNode nominant)
        {
            if (nominant.LeagueId == this.League_id &&
                nominant.AgeId == this.Age_id &&
                nominant.Block.Id == this.Block_info.Id &&
                nominant.StyleId == this.Style_id)
            {
                int index = 0;
                while (index < this.Nominants.Count && this.Nominants[index].Position < nominant.Position) index++;
                this.Nominants.Insert(index, nominant);
            }
        }

        public void AddNominantRange(IEnumerable<DanceNode> collection)
        {
            foreach(DanceNode node in collection)
            {
                this.AddNominant(node);
            }
        }

        public void SetNewType(JudgeType type)
        {
            switch (type)
            {
                case JudgeType.ThreeD:
                    if (this.JudgeIgnore.Count > 3) this.JudgeIgnore.RemoveAt(3);
                    break;
                case JudgeType.FourD:
                    if (this.JudgeIgnore.Count < 4) this.JudgeIgnore.Add(new IdCheck(4, false));
                    break;
            }
        }

        public string GetJsonJudgeIgnore()
        {
            List<bool> result = new List<bool>();
            foreach(IdCheck value in this.JudgeIgnore)
            {
                result.Add(value.IsChecked);
            }
            if (result.Count < 4) result.Add(false);

            return JsonConvert.SerializeObject(result);
        }

        public void DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Move;
        }

        public void Drop(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DanceNode node)
            {
                int oldIndex = this.Nominants.IndexOf(node);

                int insert_index = dropInfo.InsertIndex;
                if (insert_index > oldIndex) insert_index -= 1;

                this.Nominants.Move(oldIndex, insert_index);

                this.event_UpdateNominant?.Invoke(node);
            }
        }

        public void SortByNums(int step)
        {
            if (this.Nominants.Count > 1) 
            {
                for (int i = 0; i < this.Nominants.Count - 1; i++)
                {
                    for (int j = 0; j < this.Nominants.Count - i - 1; j++)
                    {
                        if (this.Nominants[j + 1].Member.MemberNum < this.Nominants[j].Member.MemberNum)
                        {
                            this.Nominants.Move(j, j + 1);
                        }
                    }
                    this.Nominants[this.Nominants.Count - i - 1].Position = this.Nominants.Count - i - 1 + step;
                }
            }
            this.Nominants.First.Position = step;

        }
    }
}
