using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Serialization;

namespace PCSX2Bonus.Legacy {
	public sealed class Game : INotifyPropertyChanged {
		public static readonly ObservableCollection<Game> AllGames = new ObservableCollection<Game>();
		private int _compatibility;
		private string _description;
		private string _imagePath;
		private bool _isVirtualized;
		private string _metacriticScore;
		private string _publisher;
		private string _releaseDate;
		private string _title;

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChanged(string property) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		public int Compatibility {
			get {
				return _compatibility;
			}
			set {
				_compatibility = value;
				OnPropertyChanged("Compatibility");
			}
		}

		public string Description {
			get {
				return _description;
			}
			set {
				_description = value;
				OnPropertyChanged("Description");
			}
		}

		public string FileSafeTitle {
			get {
				return Title.CleanFileName();
			}
		}

		public string ImagePath {
			get {
				return _imagePath;
			}
			set {
				_imagePath = value;
				OnPropertyChanged("ImagePath");
			}
		}

		public bool IsVirtualized {
			get {
				return _isVirtualized;
			}
			set {
				_isVirtualized = value;
				OnPropertyChanged("IsVirtualized");
			}
		}

		public string Location { get; set; }

		[XmlElement("Score")]
		public string MetacriticScore {
			get {
				return _metacriticScore;
			}
			set {
				_metacriticScore = value;
				OnPropertyChanged("MetacriticScore");
			}
		}

		public string Notes { get; set; }

		public string Publisher {
			get {
				return _publisher;
			}
			set {
				_publisher = value;
				OnPropertyChanged("Publisher");
			}
		}

		public string Region { get; set; }

		[XmlElement("Release")]
		public string ReleaseDate {
			get {
				return _releaseDate;
			}
			set {
				_releaseDate = value;
				OnPropertyChanged("ReleaseDate");
			}
		}

		public string Serial { get; set; }

		[XmlElement("Time")]
		public XmlSerialization.TimeSpan TimePlayed { get; set; }

		public string TimePlayedString {
			get {
				var timePlayed = (System.TimeSpan)TimePlayed;
				if (timePlayed == System.TimeSpan.Zero) {
					return "Time Played: None";
				}
				if (timePlayed.Hours > 0) {
					return string.Format("Time Played: {0} hours, {1} minutes, {2} seconds", timePlayed.Hours, timePlayed.Minutes, timePlayed.Seconds);
				}
				if (timePlayed.Minutes > 0) {
					return string.Format("Time Played: {0} minutes, {1} seconds", timePlayed.Minutes, timePlayed.Seconds);
				}
				return string.Format("Time Played: {0} seconds", timePlayed.Seconds);
			}
		}

		[XmlElement("Name")]
		public string Title {
			get {
				return _title;
			}
			set {
				_title = value;
				OnPropertyChanged("Title");
			}
		}
	}
}

