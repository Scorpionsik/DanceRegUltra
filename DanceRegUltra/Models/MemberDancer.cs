using CoreWPF.Utilites;
using DanceRegUltra.Views.EventManagerViews;
using System;

namespace DanceRegUltra.Models
{
    public class MemberDancer : Member, IComparable<MemberDancer>
    {
        private string name;
        public string Name
        {
            get => this.name;
            private set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }

        private string surname;
        public string Surname
        {
            get => this.surname;
            private set
            {
                this.surname = value;
                this.OnPropertyChanged("Surname");
            }
        }

        public MemberDancer(int eventId, int memberId, string name, string surname) : base(eventId, memberId)
        {
            this.Name = name;
            this.Surname = surname;
        }

        public void SetName(string newName)
        {
            this.Name = newName;
            this.InvokeUpdate("Name");
        }

        public void SetSurname(string newSurname)
        {
            this.Surname = newSurname;
            this.InvokeUpdate("Surname");
        }

        public int CompareTo(MemberDancer other)
        {
            return this.Surname.CompareTo(other.Surname);
        }

        private RelayCommand<MemberDancer> command_AddDancerUseMember;
        public RelayCommand<MemberDancer> Command_AddDancerUseMember
        {
            get => this.command_AddDancerUseMember;
            set
            {
                this.command_AddDancerUseMember = value;
                this.OnPropertyChanged("Command_AddDancerUseMember");
            }
        }

        private RelayCommand<MemberDancer> command_EditDancer;
        public RelayCommand<MemberDancer> Command_EditDancer
        {
            get => this.command_EditDancer;
            set
            {
                this.command_EditDancer = value;
                this.OnPropertyChanged("Command_EditDancer");
            }
        }
    }
}
