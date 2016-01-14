using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PCSX2Bonus.Legacy
{
	internal sealed class Toaster : INotifyPropertyChanged
    {
        private bool _isToasting;
        private string _message;
        private static Toaster instance = new Toaster();
        private List<string> messagePool = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public void HideToast()
        {
            this.IsToasting = false;
        }

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private async void OnToastCompleted(string sender, int duration)
        {
            if (this.messagePool.Count > 0)
            {
                await this.ShowPoolToast(this.messagePool[0], duration);
                this.messagePool.Remove(sender);
            }
        }

        public void QueueToastMessage(string message, int duration)
        {
            if (this.IsToasting)
            {
                this.messagePool.Add(message);
            }
            else
            {
                this.ShowPoolToast(message, duration);
            }
        }

        private async Task ShowPoolToast(string message, int duration)
        {
            this.Message = message;
            this.IsToasting = true;
            await Task.Delay(duration);
            this.IsToasting = false;
            this.OnToastCompleted(message, duration);
        }

        public void ShowToast(string message)
        {
            this.Message = message;
            this.IsToasting = true;
        }

        public async void ShowToast(string message, int duration)
        {
            this.Message = message;
            this.IsToasting = true;
            await Task.Delay(duration);
            this.IsToasting = false;
        }

        public static Toaster Instance
        {
            get
            {
                return instance;
            }
        }

        public bool IsToasting
        {
            get
            {
                return this._isToasting;
            }
            set
            {
                this._isToasting = value;
                this.OnPropertyChanged("IsToasting");
            }
        }

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
                this.OnPropertyChanged("Message");
            }
        }



    }
}

