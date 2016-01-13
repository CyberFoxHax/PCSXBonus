namespace PCSX2Bonus
{
    using System;
    using System.ComponentModel;
    using System.Timers;

    internal sealed class LiveDateTime : INotifyPropertyChanged
    {
        private static LiveDateTime _instance = new LiveDateTime();
        private System.Timers.Timer timer;

        public event PropertyChangedEventHandler PropertyChanged;

        public LiveDateTime()
        {
            ElapsedEventHandler handler = null;
            this.timer = new System.Timers.Timer(1000.0);
            if (handler == null)
            {
                handler = (o, e) => this.OnPropertyChanged("Now");
            }
            this.timer.Elapsed += handler;
            this.timer.Start();
        }

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public static LiveDateTime Instance
        {
            get
            {
                return _instance;
            }
        }

        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}

