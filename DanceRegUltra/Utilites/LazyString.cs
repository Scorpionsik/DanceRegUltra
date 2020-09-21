using CoreWPF.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DanceRegUltra.Utilites
{
    public class LazyString : NotifyPropertyChanged
    {
        private event Action<string> event_OnUpdateValue;
        public event Action<string> Event_OnUpdateValue
        {
            add
            {
                this.event_OnUpdateValue -= value;
                this.event_OnUpdateValue += value;
            }
            remove => this.event_OnUpdateValue -= value;
        }


        private TimerCallback Update_Callback;
        private Timer Update_Timer;
        private Func<string, string> Update_DoMethod;
        private string Identify;
        private int Timeout;
        private int Step;

        private string value;
        public string Value
        {
            get => this.value;
            set
            {
                if (this.Update_Timer != null) this.Update_Timer.Dispose();
                this.value = value;
                this.Update_Timer = new Timer(this.Update_Callback, null, this.Timeout, this.Step);
            }
        }

        public LazyString(string identify, int timeout, int step = 0, Func<string, string> method = null)
        {
            this.Identify = identify;
            this.Update_Callback = new TimerCallback(this.UpdateMethod);
            this.Timeout = timeout;
            this.Step = step;
            this.Update_DoMethod = method;
        }

        public void SetValue(string value)
        {
            this.value = value;
            this.OnPropertyChanged("Value");
        }

        private void UpdateMethod(object obj)
        {
            if (this.Update_Timer != null) this.Update_Timer.Dispose();
            if(this.Update_DoMethod != null) this.value = this.Update_DoMethod.Invoke(this.Value);
            this.OnPropertyChanged("Value");
            this.event_OnUpdateValue?.Invoke(this.Identify);
        }
    }
}
