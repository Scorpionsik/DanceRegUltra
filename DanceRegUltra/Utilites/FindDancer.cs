using CoreWPF.MVVM;
using CoreWPF.Utilites;
using DanceRegUltra.Models;
using DanceRegUltra.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanceRegUltra.Utilites
{
    public class FindDancer
    {
        private int Event_id;
        private TimerCallback Find_Callback;
        private Timer Find_Timer;
        private int Timeout;

        private event Action event_FinishSearch;
        public event Action Event_FinishSearch
        {
            add
            {
                this.event_FinishSearch -= value;
                this.event_FinishSearch += value;
            }
            remove => this.event_FinishSearch -= value;
        }

        private event Action<MemberDancer> event_changeSelectDancer;
        public event Action<MemberDancer> Event_changeSelectDancer
        {
            add
            {
                this.event_changeSelectDancer -= value;
                this.event_changeSelectDancer += value;
            }
            remove => this.event_changeSelectDancer -= value;
        }
        public ListExt<MemberDancer> FindList { get; private set; }

        private MemberDancer select_dancer;
        public MemberDancer Select_dancer
        {
            get => this.select_dancer;
            set
            {
                this.select_dancer = value;
                if(value != null) this.event_changeSelectDancer?.Invoke(value);
            }
        }

        public FindDancer(int event_id, int timeout)
        {
            this.Event_id = event_id;
            this.Timeout = timeout;
            this.FindList = new ListExt<MemberDancer>();
            this.Find_Callback = new TimerCallback(this.TimerMethod);
        }

        public void Find(string name, string surname)
        {
            if (this.Find_Timer != null) this.Find_Timer.Dispose();
            this.Find_Timer = new Timer(this.Find_Callback, new string[2] { name, surname }, this.Timeout, 0);
        }
        private void TimerMethod(object obj)
        {
            if (this.Find_Timer != null) this.Find_Timer.Dispose();
            if(obj is string[] values)
            {
                this.FindDancerAsync(values[0], values[1]);
            }
        }
        private async void FindDancerAsync(string name, string surname)
        {
            if((name != null && surname != null) && (name.Length > 0 || surname.Length > 0))
            {
                this.Select_dancer = null;
                this.FindList = new ListExt<MemberDancer>();
                string whereQuery = "";
                if(name.Length > 0)
                {
                    whereQuery += "dancers.Firstname like ('%" + App.CapitalizeAllWords(name) + "%')";
                    if (surname.Length > 0) whereQuery += " and ";
                }
                if (surname.Length > 0) whereQuery += "dancers.Surname like ('%" + App.CapitalizeAllWords(surname) + "%')";
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select dancers.Id_member, dancers.Firstname, dancers.Surname, dancers.Id_school, schools.Name from dancers join schools using (Id_school) where " + whereQuery);

                foreach(DbRow row in res)
                {
                    MemberDancer dancer = new MemberDancer(this.Event_id, row["Id_member"].ToInt32(), row["Firstname"].ToString(), row["Surname"].ToString());
                    dancer.SetSchool(DanceRegCollections.GetSchoolById(row["Id_school"].ToInt32()));
                    this.FindList.Add(dancer);
                }
                this.event_FinishSearch?.Invoke();
            }
        }

        public void Clear()
        {
            this.FindList = new ListExt<MemberDancer>();
            this.event_FinishSearch?.Invoke();
        }
    }
}
