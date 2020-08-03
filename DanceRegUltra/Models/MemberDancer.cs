namespace DanceRegUltra.Models
{
    public class MemberDancer : Member
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
    }
}
