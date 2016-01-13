namespace PCSX2Bonus
{
    using PCSX2Bonus.XmlSerialization;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Xml.Serialization;

    public sealed class Game : INotifyPropertyChanged
    {
        private static ObservableCollection<Game> _allGames = new ObservableCollection<Game>();
        private int _compatibility;
        private string _description;
        private string _imagePath;
        private bool _isVirtualized;
        private string _metacriticScore;
        private string _publisher;
        private string _releaseDate;
        private string _title;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public static ObservableCollection<Game> AllGames
        {
            get
            {
                return _allGames;
            }
        }

        public int Compatibility
        {
            get
            {
                return this._compatibility;
            }
            set
            {
                this._compatibility = value;
                this.OnPropertyChanged("Compatibility");
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
                this.OnPropertyChanged("Description");
            }
        }

        public string FileSafeTitle
        {
            get
            {
                return this.Title.CleanFileName();
            }
        }

        public string ImagePath
        {
            get
            {
                return this._imagePath;
            }
            set
            {
                this._imagePath = value;
                this.OnPropertyChanged("ImagePath");
            }
        }

        public bool IsVirtualized
        {
            get
            {
                return this._isVirtualized;
            }
            set
            {
                this._isVirtualized = value;
                this.OnPropertyChanged("IsVirtualized");
            }
        }

        public string Location { get; set; }

        [XmlElement("Score")]
        public string MetacriticScore
        {
            get
            {
                return this._metacriticScore;
            }
            set
            {
                this._metacriticScore = value;
                this.OnPropertyChanged("MetacriticScore");
            }
        }

        public string Notes { get; set; }

        public string Publisher
        {
            get
            {
                return this._publisher;
            }
            set
            {
                this._publisher = value;
                this.OnPropertyChanged("Publisher");
            }
        }

        public string Region { get; set; }

        [XmlElement("Release")]
        public string ReleaseDate
        {
            get
            {
                return this._releaseDate;
            }
            set
            {
                this._releaseDate = value;
                this.OnPropertyChanged("ReleaseDate");
            }
        }

        public string Serial { get; set; }

        [XmlElement("Time")]
        public PCSX2Bonus.XmlSerialization.TimeSpan TimePlayed { get; set; }

        public string TimePlayedString
        {
            get
            {
                System.TimeSpan timePlayed = (System.TimeSpan) this.TimePlayed;
                if (timePlayed == System.TimeSpan.Zero)
                {
                    return "Time Played: None";
                }
                if (timePlayed.Hours > 0)
                {
                    return string.Format("Time Played: {0} hours, {1} minutes, {2} seconds", timePlayed.Hours, timePlayed.Minutes, timePlayed.Seconds);
                }
                if (timePlayed.Minutes > 0)
                {
                    return string.Format("Time Played: {0} minutes, {1} seconds", timePlayed.Minutes, timePlayed.Seconds);
                }
                return string.Format("Time Played: {0} seconds", timePlayed.Seconds);
            }
        }

        [XmlElement("Name")]
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;
                this.OnPropertyChanged("Title");
            }
        }
    }
}

