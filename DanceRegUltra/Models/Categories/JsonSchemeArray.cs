using CoreWPF.MVVM;
using DanceRegUltra.Enums;
using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Windows;

namespace DanceRegUltra.Models.Categories
{
    public class JsonSchemeArray : NotifyPropertyChanged
    {
        public int IdArray { get; set; }

        private string title;
        public string Title
        {
            get => this.title;
            set
            {
                this.title = value;
                this.OnPropertyChanged("Title");
            }
        }

        public List<IdCheck> Values { get; set; }

        public JudgeType ScoreType { get; set; }

        public int JudgeCount { get; set; }

        public JsonSchemeArray()
        {
            this.Title = "";
            this.Values = new List<IdCheck>();
            this.ScoreType = JudgeType.ThreeD;
            this.JudgeCount = 0;
        }

        public JsonSchemeArray(SchemeArray sArray) : this()
        {
            this.IdArray = sArray.IdArray;
            this.Title = sArray.TitleSchemePart;
            foreach(IdCheck value in sArray.SchemePartValues)
            {
                this.Values.Add(new IdCheck(value.Id, value.IsChecked));
            }
        }

       
       

    }
}
