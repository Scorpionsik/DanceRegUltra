using CoreWPF.MVVM;
using DanceRegUltra.Enums;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static List<JudgeType> ScoreTypes { get; private set; }

        private JudgeType scoreType;
        public JudgeType ScoreType
        {
            get => this.scoreType;
            set
            {
                this.scoreType = value;
                this.OnPropertyChanged("ScoreType");
            }
        }

        private int judgeCount;
        public int JudgeCount
        {
            get => this.judgeCount;
            set
            {
                this.judgeCount = value;
                this.OnPropertyChanged("JudgeCount");
            }
        }

        static JsonSchemeArray()
        {
            ScoreTypes = new List<JudgeType>(Enum.GetValues(typeof(JudgeType)).Cast<JudgeType>());
        }

        public JsonSchemeArray()
        {
            this.Title = "";
            this.Values = new List<IdCheck>();
            this.ScoreType = JudgeType.ThreeD;
            this.JudgeCount = 1;
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
