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
    internal class FindDancer
    {

        private TimerCallback Find_Callback;
        private Timer Find_Timer;
        private int Timeout;

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

        public FindDancer(int timeout)
        {
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
                    whereQuery += "dancers.Name like (%" + name + "%)";
                    if (surname.Length > 0) whereQuery += " and ";
                }
                if (surname.Length > 0) whereQuery += "dancers.Surname like (%" + surname + "%)";
                DbResult res = await DanceRegDatabase.ExecuteAndGetQueryAsync("select dancers.Id_member, dancers.Name, dancers.Surname, dancers.Id_school, schools.Name from dancers join schools using (Id_school) where " + whereQuery);

                foreach(DbRow row in res)
                {
                    MemberDancer dancer = new MemberDancer(-1, row["dancers.Id_member"].ToInt32(), row["dancers.Name"].ToString(), row["dancers.Surname"].ToString());
                    dancer.SetSchool(new Models.Categories.IdTitle(row["dancers.Id_school"].ToInt32(), row["schools.Name"].ToString()));
                    this.FindList.Add(dancer);
                }
            }
        }


    }
}
