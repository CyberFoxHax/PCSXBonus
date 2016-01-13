namespace PCSX2Bonus
{
    using Microsoft.Win32;
    using PCSX2Bonus.Properties;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    public sealed class Commands
    {
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

        private async static void AddFromDirectory(object o)
        {
            string[] toAdd;
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                toAdd = (from s in Directory.GetFiles(fbd.SelectedPath, "*", SearchOption.AllDirectories)
                    where GameData.AcceptableFormats.Any<string>(frm => Path.GetExtension(s).Equals(frm, StringComparison.InvariantCultureIgnoreCase))
                    select s).ToArray<string>();
            }
            else
            {
                return;
            }
            await GameManager.AddGamesFromImages(toAdd);
        }

        private static void GenerateExecutable(Game g)
        {
            wndGenerateExecutable executable = new wndGenerateExecutable {
                Tag = g
            };
            bool flag1 = executable.ShowDialog() == true;
        }

        private static void GetGameManual(Game g)
        {
            string str = WebUtility.UrlEncode(g.Title);
            Process.Start(string.Format("http://www.replacementdocs.com/search.php?q={0}&r=0&s=Search&in=&ex=&ep=&be=&t=downloads&adv=1&cat=15&on=new&time=any&author=&match=0", str));
        }

        private static void OpenDonate(object o)
        {
            Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=M47HDYMNN4ZTQ&lc=US&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted");
        }

        private static void SetCoverArt(Game g)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog {
                Filter = "Image Files | *.jpg; *.png; *.gif; *.bmp"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string[] strArray = new string[] { ".jpg", ".png", ".gif", ".bmp" };
                    string str = Path.Combine(UserSettings.ImageDir, g.Title.CleanFileName());
                    string destFileName = Path.Combine(UserSettings.ImageDir, g.Title.CleanFileName() + Path.GetExtension(dialog.FileName));
                    foreach (string str3 in strArray)
                    {
                        if (System.IO.File.Exists(str + str3))
                        {
                            System.IO.File.Delete(str + str3);
                        }
                    }
                    System.IO.File.Copy(dialog.FileName, destFileName);
                    g.ImagePath = destFileName;
                }
                catch (Exception exception)
                {
                    Tools.ShowMessage("Error setting cover art: " + exception.Message, MessageType.Error);
                }
            }
        }

        private static void ShowAbout(object o)
        {
            new wndAbout().ShowDialog();
        }

        private static void ShowCustomConfig(Game g)
        {
            wndCustomConfig config = new wndCustomConfig {
                Tag = g
            };
            GameManager.ImportConfig(g);
            bool flag1 = config.ShowDialog() == true;
        }

        private static void ShowExecutableSelection(Game g)
        {
            wndExecutableSelection selection = new wndExecutableSelection {
                Tag = g
            };
            bool flag1 = selection.ShowDialog() == true;
        }

        private static void ShowMemoryCardSelection(Game g)
        {
            wndMemCard card = new wndMemCard {
                Tag = g
            };
            bool flag1 = card.ShowDialog() == true;
        }

        private static void ShowScreenshots(Game g)
        {
            wndScreenshots screenshots = new wndScreenshots {
                Tag = g
            };
            if (screenshots.ShowDialog() == true)
            {
                GC.Collect();
            }
        }

        private static void ShowSettings(object o)
        {
            wndSettings settings = new wndSettings();
            if (settings.ShowDialog() == true)
            {
                MainWindow mainWindow = (MainWindow) System.Windows.Application.Current.MainWindow;
                string defaultSort = Settings.Default.defaultSort;
                if (defaultSort != null)
                {
                    if (!(defaultSort == "Alphabetical"))
                    {
                        if (!(defaultSort == "Serial"))
                        {
                            if (!(defaultSort == "Default"))
                            {
                                return;
                            }
                            return;
                        }
                    }
                    else
                    {
                        mainWindow.ApplySort("Title", ListSortDirection.Ascending);
                        return;
                    }
                    mainWindow.ApplySort("Serial", ListSortDirection.Ascending);
                }
            }
        }

        private static void ShowShaderSelection(Game g)
        {
            wndShaderConfig config = new wndShaderConfig {
                Tag = g
            };
            bool flag1 = config.ShowDialog() == true;
        }

        public static ICommand AboutCommand
        {
            get
            {
                return (_aboutCommand ?? (_aboutCommand = new RelayCommand<object>(new Action<object>(Commands.ShowAbout))));
            }
        }

        public static ICommand AddFromDirCommand
        {
            get
            {
                return (_addfromDirCommand ?? (_addfromDirCommand = new RelayCommand<object>(new Action<object>(Commands.AddFromDirectory))));
            }
        }

        public static ICommand CoverArtCommand
        {
            get
            {
                return (_coverArtCommand ?? (_coverArtCommand = new RelayCommand<Game>(new Action<Game>(Commands.SetCoverArt))));
            }
        }

        public static ICommand CustomConfigCommand
        {
            get
            {
                return (_customConfigCommand ?? (_customConfigCommand = new RelayCommand<Game>(new Action<Game>(Commands.ShowCustomConfig))));
            }
        }

        public static ICommand DonateCommand
        {
            get
            {
                return (_donateCommand ?? (_donateCommand = new RelayCommand<object>(new Action<object>(Commands.OpenDonate))));
            }
        }

        public static ICommand GameManualCommand
        {
            get
            {
                return (_gameManualCommand ?? (_gameManualCommand = new RelayCommand<Game>(new Action<Game>(Commands.GetGameManual))));
            }
        }

        public static ICommand GenerateExecutableCommand
        {
            get
            {
                return (_generateExecutableCommand ?? (_generateExecutableCommand = new RelayCommand<Game>(new Action<Game>(Commands.GenerateExecutable))));
            }
        }

        public static ICommand MemCardCommand
        {
            get
            {
                return (_memCardCommand ?? (_memCardCommand = new RelayCommand<Game>(new Action<Game>(Commands.ShowMemoryCardSelection))));
            }
        }

        public static ICommand ScreenshotsCommand
        {
            get
            {
                return (_screenshotsCommand ?? (_screenshotsCommand = new RelayCommand<Game>(new Action<Game>(Commands.ShowScreenshots))));
            }
        }

        public static ICommand SelectExeCommand
        {
            get
            {
                return (_selectExeCommand ?? (_selectExeCommand = new RelayCommand<Game>(new Action<Game>(Commands.ShowExecutableSelection))));
            }
        }

        public static ICommand SettingsCommand
        {
            get
            {
                return (_settingsCommand ?? (_settingsCommand = new RelayCommand<object>(new Action<object>(Commands.ShowSettings))));
            }
        }

        public static ICommand ShaderCommand
        {
            get
            {
                return (_shaderCommand ?? (_shaderCommand = new RelayCommand<Game>(new Action<Game>(Commands.ShowShaderSelection))));
            }
        }

    }
}

