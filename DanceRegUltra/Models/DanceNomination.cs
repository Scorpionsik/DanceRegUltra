using CoreWPF.MVVM;
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
    public delegate void UpdateDragDrop(DanceNode node, int old_index, int new_index);

    public class DanceNomination : NotifyPropertyChanged, IDropTarget
    {
        private event UpdateDragDrop event_UpdateNominant;
        public event UpdateDragDrop Event_UpdateNominant
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

        private DanceNode select_nominant;
        public DanceNode Select_nominant
        {
            get => this.select_nominant;
            set
            {
                this.select_nominant = value;
                this.OnPropertyChanged("Select_nominant");
            }
        }

        private JudgeType type;
        public JudgeType Type
        {
            get => this.type;
            private set
            {
                this.type = value;
                this.OnPropertyChanged("Type");
            }
        }

        public int JudgeCount
        {
            get => this.JudgeIgnore != null ? this.JudgeIgnore.Count : 0;
        }

        private bool separate_dancer_group;
        public bool Separate_dancer_group
        {
            get => this.separate_dancer_group;
            private set
            {
                this.separate_dancer_group = value;
                this.OnPropertyChanged("Separate_dancer_group");
            }
        }

        public ListExt<IdCheck> JudgeIgnore { get; private set; }

        public DanceNomination(int event_id, IdTitle block, JudgeType type, int league, int age, int style, bool isShow, string jsonJudge = "", bool separate = true)
        {
            this.Event_id = event_id;
            this.League_id = league;
            this.Age_id = age;
            this.Block_info = new IdTitle(block.Id, block.Title);
            this.Style_id = style;
            this.IsShowInList = isShow;
            this.Separate_dancer_group = separate;
            this.Nominants = new ListExt<DanceNode>();
            this.JudgeIgnore = new ListExt<IdCheck>();
            this.JudgeIgnore.CollectionChanged += this.UpdateJudgeCount;
            List<bool> tmp_judge = jsonJudge == "" ? new List<bool>() : JsonConvert.DeserializeObject<List<bool>>(jsonJudge);
            int index = 0;
            while(index < tmp_judge.Count)
            {
                this.JudgeIgnore.Add(new IdCheck(index + 1, tmp_judge[index]));
                index++;
            }

            this.SetNewType(type);
            /*
            for(int i = 0; i < 4; i++)
            {
                if (i == 3 && type == JudgeType.ThreeD) continue;
                this.JudgeIgnore.Add(new IdCheck(i + 1, tmp_judge[i]));
            }*/
        }

        private void UpdateJudgeCount(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnPropertyChanged("JudgeCount");
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

        /// <summary>
        /// Проверка, все ли узлы оценены, и выставление призовых мест
        /// </summary>
        public async Task<bool> CheckNodeScores()
        {
            return await Task<bool>.Run(() =>
            {
                Dictionary<DanceNode, double> averages = new Dictionary<DanceNode, double>();
                foreach (DanceNode node in this.Nominants)
                {
                    double average = node.GetAverage(this.JudgeIgnore, this.Type, this.Separate_dancer_group);
                    if (average <= 0) node.SetPrizePlace(0);
                    else averages.Add(node, average);
                }

                List<DanceNode> tmp_nominants = new List<DanceNode>(averages.Keys);

                for (int i = 0; i < tmp_nominants.Count; i++)
                {
                    for (int j = i; j < tmp_nominants.Count - 1; j++)
                    {
                        if (averages[tmp_nominants[j + 1]] > averages[tmp_nominants[j]])
                        {
                            DanceNode tmp_node = tmp_nominants[j];
                            tmp_nominants[j] = tmp_nominants[j + 1];
                            tmp_nominants[j + 1] = tmp_node;
                        }
                    }
                }

                for (int i = 0, prize = 1; i < tmp_nominants.Count; i++, prize++)
                {
                    tmp_nominants[i].SetPrizePlace(prize);
                    if (i != tmp_nominants.Count - 1 && averages[tmp_nominants[i]] == averages[tmp_nominants[i + 1]]) prize--;
                }
                return true;
            });
        }

        public void SetNewType(JudgeType type)
        {
            /*
            switch (type)
            {
                case JudgeType.ThreeD:
                    if (this.JudgeIgnore.Count > 3) this.JudgeIgnore.RemoveAt(3);
                    break;
                case JudgeType.FourD:
                    if (this.JudgeIgnore.Count < 4) this.JudgeIgnore.Add(new IdCheck(4, false));
                    break;
            }*/
            this.Type = type;
        }

        public void SetJudgeCount(int count)
        {
            
            while(count != this.JudgeIgnore.Count)
            {
                if (count < this.JudgeIgnore.Count)
                {
                    this.JudgeIgnore.RemoveAt(this.JudgeIgnore.Count - 1);
                }
                else this.JudgeIgnore.Add(new IdCheck(this.JudgeIgnore.Count + 1, false));
            }
        }

        public void SetSeparate(bool separate)
        {
            this.Separate_dancer_group = separate;
        }

        public string GetJsonJudgeIgnore()
        {
            List<bool> result = new List<bool>();
            foreach(IdCheck value in this.JudgeIgnore)
            {
                result.Add(value.IsChecked);
            }
            //if (result.Count < 4) result.Add(false);

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
                if (oldIndex > -1)
                {
                    int insert_index = dropInfo.InsertIndex;
                    if (insert_index > oldIndex) insert_index -= 1;

                    this.Nominants.Move(oldIndex, insert_index);

                    this.event_UpdateNominant?.Invoke(node, oldIndex, insert_index);
                }
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

        private RelayCommand<DanceNomination> command_AddDancerUseNomination;
        public RelayCommand<DanceNomination> Command_AddDancerUseNomination
        {
            get => this.command_AddDancerUseNomination;
            set
            {
                this.command_AddDancerUseNomination = value;
                this.OnPropertyChanged("Command_AddDancerUseNomination");
            }
        }

        private RelayCommand<DanceNomination> command_AddGroupUseNomination;
        public RelayCommand<DanceNomination> Command_AddGroupUseNomination
        {
            get => this.command_AddGroupUseNomination;
            set
            {
                this.command_AddGroupUseNomination = value;
                this.OnPropertyChanged("Command_AddGroupUseNomination");
            }
        }

        private RelayCommand<DanceNomination> command_DeleteNodesByNomination;
        public RelayCommand<DanceNomination> Command_DeleteNodesByNomination
        {
            get => this.command_DeleteNodesByNomination;
            set
            {
                this.command_DeleteNodesByNomination = value;
                this.OnPropertyChanged("Command_DeleteNodesByNomination");
            }
        }

        private RelayCommand<DanceNomination> command_editSelectNomination;
        public RelayCommand<DanceNomination> Command_editSelectNomination
        {
            get => this.command_editSelectNomination;
            set
            {
                this.command_editSelectNomination = value;
                this.OnPropertyChanged("Command_editSelectNomination");
            }
        }

        private RelayCommand<DanceNomination> command_ChangeBlockForNomination;
        public RelayCommand<DanceNomination> Command_ChangeBlockForNomination
        {
            get => this.command_ChangeBlockForNomination;
            set
            {
                this.command_ChangeBlockForNomination = value;
                this.OnPropertyChanged("Command_ChangeBlockForNomination");
            }
        }

        private RelayCommand<DanceNode> command_NodeSetScore;
        public RelayCommand<DanceNode> Command_NodeSetScore
        {
            get => this.command_NodeSetScore;
            set
            {
                this.command_NodeSetScore = value;
                this.OnPropertyChanged("Command_NodeSetScore");
            }
        }
    }
}
