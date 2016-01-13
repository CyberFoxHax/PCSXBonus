namespace PCSX2Bonus {
	using Properties;
	using System;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Windows.Forms;
	using System.Windows.Input;

	public sealed class Commands {
		private static ICommand _aboutCommand;
		private static ICommand _addfromDirCommand;
		private static ICommand _coverArtCommand;
		private static ICommand _customConfigCommand;
		private static ICommand _donateCommand;
		private static ICommand _gameManualCommand;
		private static ICommand _generateExecutableCommand;
		private static ICommand _memCardCommand;
		private static ICommand _screenshotsCommand;
		private static ICommand _selectExeCommand;
		private static ICommand _settingsCommand;
		private static ICommand _shaderCommand;

		private async static void AddFromDirectory(object o) {
			string[] toAdd;
			var fbd = new FolderBrowserDialog();
			if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				toAdd = (from s in Directory.GetFiles(fbd.SelectedPath, "*", SearchOption.AllDirectories)
					let extension = Path.GetExtension(s)
					where GameData.AcceptableFormats.Any(frm => extension != null && extension.Equals(frm, StringComparison.InvariantCultureIgnoreCase))
					select s
				).ToArray<string>();
			}
			else {
				return;
			}
			await GameManager.AddGamesFromImages(toAdd);
		}

		private static void GenerateExecutable(Game g) {
			var executable = new wndGenerateExecutable {
				Tag = g
			};
			var flag1 = executable.ShowDialog() == true;
		}

		private static void GetGameManual(Game g) {
			var str = WebUtility.UrlEncode(g.Title);
			Process.Start(string.Format("http://www.replacementdocs.com/search.php?q={0}&r=0&s=Search&in=&ex=&ep=&be=&t=downloads&adv=1&cat=15&on=new&time=any&author=&match=0", str));
		}

		private static void OpenDonate(object o) {
			Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=M47HDYMNN4ZTQ&lc=US&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted");
		}

		private static void SetCoverArt(Game g) {
			var dialog = new Microsoft.Win32.OpenFileDialog {
				Filter = "Image Files | *.jpg; *.png; *.gif; *.bmp"
			};
			if (dialog.ShowDialog() == true) {
				try {
					var strArray = new [] { ".jpg", ".png", ".gif", ".bmp" };
					var str = Path.Combine(UserSettings.ImageDir, g.Title.CleanFileName());
					var destFileName = Path.Combine(UserSettings.ImageDir, g.Title.CleanFileName() + Path.GetExtension(dialog.FileName));
					foreach (var str3 in strArray) {
						if (File.Exists(str + str3)) {
							File.Delete(str + str3);
						}
					}
					File.Copy(dialog.FileName, destFileName);
					g.ImagePath = destFileName;
				}
				catch (Exception exception) {
					Tools.ShowMessage("Error setting cover art: " + exception.Message, MessageType.Error);
				}
			}
		}

		private static void ShowAbout(object o) {
			new wndAbout().ShowDialog();
		}

		private static void ShowCustomConfig(Game g) {
			var config = new wndCustomConfig {
				Tag = g
			};
			GameManager.ImportConfig(g);
			var flag1 = config.ShowDialog() == true;
		}

		private static void ShowExecutableSelection(Game g) {
			var selection = new wndExecutableSelection {
				Tag = g
			};
			var flag1 = selection.ShowDialog() == true;
		}

		private static void ShowMemoryCardSelection(Game g) {
			var card = new wndMemCard {
				Tag = g
			};
			var flag1 = card.ShowDialog() == true;
		}

		private static void ShowScreenshots(Game g) {
			var screenshots = new wndScreenshots {
				Tag = g
			};
			if (screenshots.ShowDialog() == true)
				GC.Collect();
		}

		private static void ShowSettings(object o) {
			var settings = new wndSettings();
			if (settings.ShowDialog() != true) return;
			var mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
			var defaultSort = Settings.Default.defaultSort;
			if (defaultSort == null) return;
			if (defaultSort != "Alphabetical") {
				if (defaultSort != "Serial") {
					if (defaultSort != "Default")
						return;
					return;
				}
			}
			else {
				mainWindow.ApplySort("Title", ListSortDirection.Ascending);
				return;
			}
			mainWindow.ApplySort("Serial", ListSortDirection.Ascending);
		}

		private static void ShowShaderSelection(Game g) {
			var config = new wndShaderConfig {
				Tag = g
			};
			var flag1 = config.ShowDialog() == true;
		}

		public static ICommand AboutCommand {
			get {
				return (_aboutCommand ?? (_aboutCommand = new RelayCommand<object>(ShowAbout)));
			}
		}

		public static ICommand AddFromDirCommand {
			get {
				return (_addfromDirCommand ?? (_addfromDirCommand = new RelayCommand<object>(AddFromDirectory)));
			}
		}

		public static ICommand CoverArtCommand {
			get {
				return (_coverArtCommand ?? (_coverArtCommand = new RelayCommand<Game>(SetCoverArt)));
			}
		}

		public static ICommand CustomConfigCommand {
			get {
				return (_customConfigCommand ?? (_customConfigCommand = new RelayCommand<Game>(ShowCustomConfig)));
			}
		}

		public static ICommand DonateCommand {
			get {
				return (_donateCommand ?? (_donateCommand = new RelayCommand<object>(OpenDonate)));
			}
		}

		public static ICommand GameManualCommand {
			get {
				return (_gameManualCommand ?? (_gameManualCommand = new RelayCommand<Game>(GetGameManual)));
			}
		}

		public static ICommand GenerateExecutableCommand {
			get {
				return (_generateExecutableCommand ?? (_generateExecutableCommand = new RelayCommand<Game>(GenerateExecutable)));
			}
		}

		public static ICommand MemCardCommand {
			get {
				return (_memCardCommand ?? (_memCardCommand = new RelayCommand<Game>(ShowMemoryCardSelection)));
			}
		}

		public static ICommand ScreenshotsCommand {
			get {
				return (_screenshotsCommand ?? (_screenshotsCommand = new RelayCommand<Game>(ShowScreenshots)));
			}
		}

		public static ICommand SelectExeCommand {
			get {
				return (_selectExeCommand ?? (_selectExeCommand = new RelayCommand<Game>(ShowExecutableSelection)));
			}
		}

		public static ICommand SettingsCommand {
			get {
				return (_settingsCommand ?? (_settingsCommand = new RelayCommand<object>(ShowSettings)));
			}
		}

		public static ICommand ShaderCommand {
			get {
				return (_shaderCommand ?? (_shaderCommand = new RelayCommand<Game>(ShowShaderSelection)));
			}
		}

	}
}

