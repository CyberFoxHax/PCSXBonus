using System;
using System.ComponentModel;
using System.Timers;

namespace PCSX2Bonus.Legacy {
	internal sealed class LiveDateTime : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		public LiveDateTime() {
			var timer = new Timer(1000.0);
			ElapsedEventHandler handler = (o, e) => OnPropertyChanged("Now");
			timer.Elapsed += handler;
			timer.Start();
		}

		private void OnPropertyChanged(string property) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public DateTime Now {
			get {
				return DateTime.Now;
			}
		}
	}
}

