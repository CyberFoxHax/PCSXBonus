namespace PCSX2Bonus
{
    using Microsoft.Win32;
    using PCSX2Bonus.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;
    using System.Windows.Threading;
    using System.Xml.Linq;

    public sealed class MainWindow : Window, IComponentConnector, IStyleConnector
    {
        private bool _contentLoaded;
        private NotifyIcon _notifyIcon = new NotifyIcon();
        private DispatcherTimer _t;
        internal System.Windows.Controls.Button btnSaveWidePatch;
        internal ImageButton btnStacked;
        internal ImageButton btnTile;
        internal ImageButton btnTV;
        internal Grid gWideScreenResults;
        internal Image imgPreview;
        internal System.Windows.Controls.ListView lvGames;
        internal System.Windows.Controls.ListView lvSaveStates;
        internal System.Windows.Controls.ListView lvScrape;
        internal System.Windows.Controls.MenuItem miAddFromImage;
        internal System.Windows.Controls.MenuItem miRemoveStates;
        internal System.Windows.Controls.RichTextBox rtbResults;
        internal System.Windows.Controls.TextBox tbDebug;
        internal TextBlock tbInfo;
        internal System.Windows.Controls.TextBox tbSearch;

        public MainWindow()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
            base.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            this.CheckSettings();
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        internal Delegate _CreateDelegate(System.Type delegateType, string handler)
        {
            return Delegate.CreateDelegate(delegateType, this, handler);
        }

        private void AddDummies()
        {
            int num = 500;
            for (int i = num; i < (num + 100); i++)
            {
                int result = 0;
                string str = GameManager.GameDatabase[i];
                string[] source = str.Trim().Split(new char[] { '\n' });
                string str2 = source.FirstOrDefault<string>(s => s.Contains("Compat"));
                if (!string.IsNullOrWhiteSpace(str2))
                {
                    result = int.TryParse(str2.Replace("Compat = ", ""), out result) ? result : 0;
                }
                Game g = new Game {
                    Serial = source[0],
                    Title = source[1],
                    Region = source[2],
                    Description = "n/a*",
                    Location = "",
                    ImagePath = "",
                    Compatibility = result
                };
                if (!Game.AllGames.Any<Game>(game => (game.Serial == g.Serial)))
                {
                    GameManager.AddToLibrary(g);
                }
            }
        }

        private async void AddFromImageFile()
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog {
                Multiselect = true
            };
            new List<Game>();
            if (ofd.ShowDialog() == true)
            {
                await GameManager.AddGamesFromImages(ofd.FileNames);
            }
        }

        public void ApplySort(string category, ListSortDirection direction)
        {
            this.lvGames.Items.SortDescriptions.Clear();
            this.lvGames.Items.SortDescriptions.Add(new SortDescription(category, direction));
        }

        private void ApplyWindowSettings()
        {
            if (Settings.Default.windowSize != Size.Empty)
            {
                base.Width = Settings.Default.windowSize.Width;
                base.Height = Settings.Default.windowSize.Height;
            }
            base.WindowState = (WindowState) Enum.Parse(typeof(WindowState), Settings.Default.windowState);
            base.Left = Settings.Default.windowLocation.X;
            base.Top = Settings.Default.windowLocation.Y;
        }

        private void CheckSettings()
        {
            if ((Settings.Default.pcsx2Dir.IsEmpty() || Settings.Default.pcsx2DataDir.IsEmpty()) || (!Directory.Exists(Settings.Default.pcsx2Dir) || !Directory.Exists(Settings.Default.pcsx2DataDir)))
            {
                Tools.ShowMessage("Please select the directory containing PCSX2 and the PCSX2 data directory", MessageType.Info);
                new wndSetup().ShowDialog();
            }
        }

        private void ClearViewStates(ImageButton sender)
        {
            this.btnStacked.IsChecked = false;
            this.btnTile.IsChecked = false;
            this.btnTV.IsChecked = false;
            sender.IsChecked = true;
        }

        private void crashMe()
        {
            object obj2 = null;
            obj2.ToString();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exceptionObject = (Exception) e.ExceptionObject;
            new wndErrorReport { Message = exceptionObject.Message, StackTrace = exceptionObject.StackTrace, Owner = this }.ShowDialog();
        }

        private void DeleteSaveStates()
        {
            if (this.lvSaveStates.SelectedItems.Count != 0)
            {
                List<SaveState> list = this.lvSaveStates.ItemsSource.Cast<SaveState>().ToList<SaveState>();
                foreach (object obj2 in this.lvSaveStates.SelectedItems)
                {
                    SaveState item = (SaveState) obj2;
                    try
                    {
                        File.Delete(item.Location);
                        list.Remove(item);
                    }
                    catch
                    {
                    }
                }
                this.lvSaveStates.ItemsSource = list;
            }
        }

        private void Filter(object sender, EventArgs e)
        {
            string query;
            this._t.Stop();
            if (this.lvGames.IsVisible)
            {
                query = this.tbSearch.Text;
                ICollectionView defaultView = CollectionViewSource.GetDefaultView(Game.AllGames);
                if (query.IsEmpty() || (query == "Search"))
                {
                    if (defaultView.Filter != null)
                    {
                        defaultView.Filter = null;
                    }
                }
                else
                {
                    defaultView.Filter = (Predicate<object>) Delegate.Combine(defaultView.Filter, o => ((Game) o).Title.Contains(query, StringComparison.InvariantCultureIgnoreCase));
                    ((ScrollViewer) Tools.GetDescendantByType(this.lvGames, typeof(ScrollViewer))).ScrollToVerticalOffset(0.0);
                }
            }
        }

        private async void HideRescrape()
        {
            this.lvScrape.IsHitTestVisible = false;
            this.lvGames.Visibility = Visibility.Visible;
            await this.SlideIn();
            this.lvScrape.Visibility = Visibility.Collapsed;
        }

        private async void HideSaveStates()
        {
            await this.SlideIn();
            this.lvSaveStates.Visibility = Visibility.Collapsed;
            this.lvSaveStates.IsHitTestVisible = false;
        }

        private async void HideWidescreenResults()
        {
            await this.SlideIn();
            this.gWideScreenResults.Visibility = Visibility.Collapsed;
            this.gWideScreenResults.IsHitTestVisible = false;
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/mainwindow.xaml", UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocator);
            }
        }

        public void LaunchGame(Game g, bool tvMode = false)
        {
            Process p;
            DateTime _timeOpened;
            string str = string.Empty;
            if (!File.Exists(g.Location))
            {
                Tools.ShowMessage("Unable to find image file!", MessageType.Error);
                if (tvMode)
                {
                    Window window = System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault<Window>(w => w.Title == "wndFullScreen");
                    if (window != null)
                    {
                        window.ShowDialog();
                    }
                }
            }
            else
            {
                p = new Process();
                string path = UserSettings.RootDir + string.Format(@"\Configs\{0}", g.FileSafeTitle);
                if (Directory.Exists(path))
                {
                    str = string.Format(" --cfgpath={0}{2}{0} {0}{1}{0}", "\"", g.Location, path);
                }
                else
                {
                    str = string.Format(" {0}{1}{0}", "\"", g.Location);
                }
                str = str.Replace(@"\\", @"\");
                string src = string.Empty;
                string str4 = Settings.Default.pcsx2Dir;
                if (File.Exists(path + @"\PCSX2Bonus.ini"))
                {
                    IniFile file = new IniFile(path + @"\PCSX2Bonus.ini");
                    string str5 = file.Read("Additional Executables", "Default");
                    string str6 = !str5.IsEmpty() ? str5 : Settings.Default.pcsx2Exe;
                    str4 = !str5.IsEmpty() ? Path.GetDirectoryName(str5) : Settings.Default.pcsx2Dir;
                    string str7 = file.Read("Boot", "NoGUI");
                    string str8 = file.Read("Boot", "UseCD");
                    string str9 = file.Read("Boot", "NoHacks");
                    string str10 = file.Read("Boot", "FullBoot");
                    src = file.Read("Shader", "Default");
                    p.StartInfo.FileName = str6;
                    if (str7 == "true")
                    {
                        str = str.Insert(0, " --nogui");
                    }
                    if (str8 == "true")
                    {
                        str = str.Insert(0, " --usecd");
                    }
                    if (str9 == "true")
                    {
                        str = str.Insert(0, " --nohacks");
                    }
                    if (str10 == "true")
                    {
                        str = str.Insert(0, " --fullboot");
                    }
                }
                else
                {
                    p.StartInfo.FileName = Settings.Default.pcsx2Exe;
                }
                p.EnableRaisingEvents = true;
                p.StartInfo.WorkingDirectory = str4;
                p.StartInfo.Arguments = str;
                if (!src.IsEmpty())
                {
                    try
                    {
                        File.Copy(src, Path.Combine(str4, "shader.fx"), true);
                    }
                    catch (Exception exception)
                    {
                        Tools.ShowMessage("Could not save shader file! Details: " + exception.Message, MessageType.Error);
                    }
                }
                _timeOpened = DateTime.Now;
                p.Exited += delegate (object o, EventArgs e) {
                    if (p != null)
                    {
                        p.Dispose();
                    }
                    System.Windows.Application.Current.Dispatcher.Invoke(delegate {
                        DateTime now = DateTime.Now;
                        g.TimePlayed = g.TimePlayed.Add(now.Subtract(_timeOpened));
                        XElement element = UserSettings.xGames.Descendants("Game").FirstOrDefault<XElement>(x => x.Element("Name").Value == g.Title);
                        if ((element != null) && (element.Element("Time") != null))
                        {
                            element.Element("Time").Value = g.TimePlayed.ToString();
                        }
                        if (tvMode)
                        {
                            Window window = System.Windows.Application.Current.Windows.Cast<Window>().FirstOrDefault<Window>(w => w.Title == "wndFullScreen");
                            if (window != null)
                            {
                                window.ShowDialog();
                            }
                        }
                        this._notifyIcon.Visible = false;
                        this.Show();
                        this.Activate();
                    });
                };
                base.Hide();
                if (Settings.Default.enableGameToast)
                {
                    new wndGameNotify { Tag = g }.Show();
                }
                this._notifyIcon.Text = string.Format("Currently playing [{0}]", g.Title.Truncate(40));
                this._notifyIcon.Visible = true;
                p.Start();
            }
        }

        private void lvGames_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (this.lvGames.SelectedItems.Count == 0)
            {
                e.Handled = true;
            }
            else if (this.lvGames.SelectedItems.Count == 1)
            {
                foreach (object obj2 in (IEnumerable) this.lvGames.ContextMenu.Items)
                {
                    FrameworkElement element = (FrameworkElement) obj2;
                    element.Visibility = Visibility.Visible;
                }
            }
            else if (this.lvGames.SelectedItems.Count > 1)
            {
                foreach (object obj3 in (IEnumerable) this.lvGames.ContextMenu.Items)
                {
                    FrameworkElement element2 = (FrameworkElement) obj3;
                    if (element2.Name == "miRemove")
                    {
                        element2.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        element2.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void lvGames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int count = this.lvGames.SelectedItems.Count;
            if ((count <= 1) && (count != 0))
            {
                this.lvGames.Tag = (Game) this.lvGames.SelectedItem;
                System.Windows.Controls.ListViewItem root = (System.Windows.Controls.ListViewItem) this.lvGames.ItemContainerGenerator.ContainerFromItem(this.lvGames.SelectedItem);
                Image image = Tools.FindByName("imgGameCover", root) as Image;
                if (image != null)
                {
                    this.imgPreview.Source = image.Source;
                }
            }
        }

        public void lviDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.ListViewItem source = (System.Windows.Controls.ListViewItem) e.Source;
            Game dataContext = (Game) source.DataContext;
            this.LaunchGame(dataContext, false);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Setup();
        }

        private void MainWindow_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!this.lvGames.IsVisible && this.lvScrape.IsVisible)
                {
                    this.HideRescrape();
                }
                else if (!this.lvGames.IsVisible && this.lvSaveStates.IsVisible)
                {
                    this.HideSaveStates();
                }
                else if (!this.lvGames.IsVisible && this.gWideScreenResults.IsVisible)
                {
                    this.HideWidescreenResults();
                }
            }
            else if (e.Key == Key.Oem3)
            {
                if (this.tbDebug.Visibility == Visibility.Collapsed)
                {
                    this.tbDebug.Text = string.Empty;
                    this.tbDebug.Visibility = Visibility.Visible;
                    this.tbDebug.Focus();
                    e.Handled = true;
                }
                else if (this.tbDebug.Visibility == Visibility.Visible)
                {
                    this.tbDebug.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void miAddFromImage_Click(object sender, RoutedEventArgs e)
        {
            this.AddFromImageFile();
        }

        private void miRescrape_Click(object sender, RoutedEventArgs e)
        {
            this.ShowScrapeResults();
        }

        private void RemoveSelectedGames(List<Game> games)
        {
            foreach (Game game in games)
            {
                GameManager.Remove(game);
            }
        }

        private async void Rescrape()
        {
            GameSearchResult selectedItem = (GameSearchResult) this.lvScrape.SelectedItem;
            Game tag = (Game) this.lvGames.Tag;
            MessageBoxResult mbr = System.Windows.MessageBox.Show(string.Format("Are you sure you want to update the game {0} to {1}?", tag.Title, selectedItem.Name), "PCSX2Bonus - Confirm Game Update", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr != MessageBoxResult.No)
            {
                this.lvScrape.IsHitTestVisible = false;
                Toaster.Instance.ShowToast("Fetching info for: " + selectedItem.Name);
            }
            else
            {
                return;
            }
            await GameManager.ReFetchInfo(tag, selectedItem.Link);
            Toaster.Instance.ShowToast("Successfully updated info for: " + tag.Title, 0xbb8);
            this.HideRescrape();
        }

        private void SaveWindowSettings()
        {
            if (base.WindowState != WindowState.Minimized)
            {
                Settings.Default.windowLocation = new Point(base.Left, base.Top);
                Settings.Default.windowSize = new Size(base.Width, base.Height);
                Settings.Default.windowState = WindowState.Normal.ToString();
            }
            else if (base.WindowState == WindowState.Maximized)
            {
                Settings.Default.windowState = WindowState.Maximized.ToString();
            }
        }

        private async void Setup()
        {
            this.ApplyWindowSettings();
            this.tbDebug.KeyDown += new System.Windows.Input.KeyEventHandler(this.tbDebug_KeyDown);
            this.SetupNotifyIcon();
            this.lvGames.SelectionChanged += new SelectionChangedEventHandler(this.lvGames_SelectionChanged);
            System.Windows.Controls.ContextMenu contextMenu = this.lvGames.ContextMenu;
            NameScope.SetNameScope(contextMenu, NameScope.GetNameScope(this));
            System.Windows.Controls.MenuItem miRemove = (System.Windows.Controls.MenuItem) contextMenu.Items[1];
            System.Windows.Controls.MenuItem miRescrape = (System.Windows.Controls.MenuItem) contextMenu.Items[3];
            System.Windows.Controls.MenuItem miSaveStates = (System.Windows.Controls.MenuItem) contextMenu.Items[4];
            System.Windows.Controls.MenuItem miWideScreen = (System.Windows.Controls.MenuItem) contextMenu.Items[13];
            System.Windows.Controls.MenuItem miPlay = (System.Windows.Controls.MenuItem) contextMenu.Items[0];
            miRescrape.Click += new RoutedEventHandler(this.miRescrape_Click);
            this.lvGames.ContextMenuOpening += new ContextMenuEventHandler(this.lvGames_ContextMenuOpening);
            this.miRemoveStates.Click += (o, e) => this.DeleteSaveStates();
            this.lvGames.AllowDrop = true;
            this.lvGames.Drop += async delegate (object o, System.Windows.DragEventArgs e) {
                string[] data = (string[]) e.Data.GetData(System.Windows.DataFormats.FileDrop);
                string[] files = (from s in data
                    where GameData.AcceptableFormats.Any<string>(frm => Path.GetExtension(s).Equals(frm, StringComparison.InvariantCultureIgnoreCase))
                    select s).ToArray<string>();
                await GameManager.AddGamesFromImages(files);
            };
            miPlay.Click += delegate (object o, RoutedEventArgs e) {
                int count = this.lvGames.SelectedItems.Count;
                if ((count != 0) && (count <= 1))
                {
                    this.LaunchGame((Game) this.lvGames.SelectedItem, false);
                }
            };
            miRemove.Click += delegate (object o, RoutedEventArgs e) {
                List<Game> games = this.lvGames.SelectedItems.Cast<Game>().ToList<Game>();
                this.RemoveSelectedGames(games);
            };
            miSaveStates.Click += (o, e) => this.ShowSaveStates();
            miWideScreen.Click += (o, e) => this.ShowWideScreenResults();
            this._t = new DispatcherTimer(TimeSpan.FromMilliseconds(200.0), DispatcherPriority.DataBind, new EventHandler(this.Filter), this.Dispatcher);
            this.btnSaveWidePatch.Click += async delegate (object o, RoutedEventArgs e) {
                Game g = (Game) this.lvGames.Tag;
                if (g != null)
                {
                    string crc = await GameManager.FetchCRC(g);
                    if (!Directory.Exists(Settings.Default.pcsx2Dir + @"\Cheats"))
                    {
                        Directory.CreateDirectory(Settings.Default.pcsx2Dir + @"\Cheats");
                    }
                    string contents = new TextRange(this.rtbResults.Document.ContentStart, this.rtbResults.Document.ContentEnd).Text;
                    try
                    {
                        File.WriteAllText(Settings.Default.pcsx2Dir + @"\Cheats\" + crc + ".pnach", contents);
                        Toaster.Instance.ShowToast(string.Format("Successfully saved patch for {0} as {1}.pnatch", g.Title, crc), 0xdac);
                        this.HideWidescreenResults();
                    }
                    catch (Exception exception)
                    {
                        Toaster.Instance.ShowToast("An error occured when trying to save the patch: " + exception.Message);
                    }
                }
            };
            this.btnStacked.Click += delegate (object o, RoutedEventArgs e) {
                this.ClearViewStates(this.btnStacked);
                this.SwitchView("gridView");
            };
            this.btnTile.Click += delegate (object o, RoutedEventArgs e) {
                this.ClearViewStates(this.btnTile);
                this.SwitchView("tileView");
            };
            this.btnTV.Click += (o, e) => this.SwitchView("tv");
            this.lvGames.MouseDown += delegate (object o, MouseButtonEventArgs e) {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.lvGames.UnselectAll();
                }
            };
            this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.MainWindow_PreviewKeyDown);
            this.lvGames.MouseDown += delegate (object o, MouseButtonEventArgs e) {
                if (e.MiddleButton == MouseButtonState.Pressed)
                {
                    this.lvGames.UnselectAll();
                }
            };
            this.lvScrape.MouseDoubleClick += delegate (object o, MouseButtonEventArgs e) {
                int count = this.lvScrape.SelectedItems.Count;
                if ((count != 0) && (count <= 1))
                {
                    this.Rescrape();
                }
            };
            this.tbSearch.GotFocus += delegate (object o, RoutedEventArgs e) {
                if ((this.tbSearch.Text == "Search") || this.tbSearch.Text.IsEmpty())
                {
                    this.tbSearch.Text = string.Empty;
                }
            };
            this.tbSearch.LostFocus += delegate (object o, RoutedEventArgs e) {
                if (this.tbSearch.Text.IsEmpty())
                {
                    this.tbSearch.Text = "Search";
                }
            };
            this.tbSearch.TextChanged += new TextChangedEventHandler(this.tbSearch_TextChanged);
            this.tbSearch.KeyDown += delegate (object o, System.Windows.Input.KeyEventArgs e) {
                if (e.Key == Key.Escape)
                {
                    this.tbSearch.Text = "Search";
                    this.lvGames.Focus();
                }
            };
            await GameManager.BuildDatabase();
            GameManager.GenerateDirectories();
            GameManager.LoadXml();
            Console.WriteLine("loaded xml");
            await GameManager.GenerateUserLibrary();
            GameManager.UpdateGamesToLatestCompatibility();
            string defaultView = Settings.Default.defaultView;
            if (defaultView != null)
            {
                if (!(defaultView == "Stacked"))
                {
                    if (defaultView == "Tile")
                    {
                        this.SwitchView("tileView");
                        this.ClearViewStates(this.btnTile);
                    }
                    else if (defaultView == "TV")
                    {
                        this.SwitchView("tv");
                        this.ClearViewStates(this.btnStacked);
                    }
                }
                else
                {
                    this.SwitchView("gridView");
                    this.ClearViewStates(this.btnStacked);
                }
            }
            string defaultSort = Settings.Default.defaultSort;
            if (defaultSort != null)
            {
                if (!(defaultSort == "Alphabetical"))
                {
                    if (defaultSort == "Serial")
                    {
                        this.ApplySort("Serial", ListSortDirection.Ascending);
                    }
                    else if (defaultSort == "Default")
                    {
                    }
                }
                else
                {
                    this.ApplySort("Title", ListSortDirection.Ascending);
                }
            }
        }

        private void SetupNotifyIcon()
        {
            this._notifyIcon.Icon = Resources.icon;
        }

        private async void ShowSaveStates()
        {
            Game selectedItem = (Game) this.lvGames.SelectedItem;
            if (selectedItem != null)
            {
                List<SaveState> states = GameManager.FetchSaveStates(selectedItem);
                if (states.Count != 0)
                {
                    this.lvSaveStates.Visibility = Visibility.Visible;
                    this.lvSaveStates.ItemsSource = states;
                    this.lvSaveStates.IsHitTestVisible = true;
                    this.lvGames.UnselectAll();
                    await this.SlideOut();
                }
                else
                {
                    Toaster.Instance.ShowToast("No save states found", 0x9c4);
                }
            }
        }

        private async void ShowScrapeResults()
        {
            this.lvScrape.ItemsSource = null;
            Game selectedItem = (Game) this.lvGames.SelectedItem;
            if (selectedItem != null)
            {
                this.lvScrape.Visibility = Visibility.Visible;
                this.lvScrape.IsHitTestVisible = true;
                this.lvGames.UnselectAll();
                this.lvGames.IsHitTestVisible = false;
                Toaster.Instance.ShowToast("Fetching results for: " + selectedItem.Title);
                List<GameSearchResult> results = await GameManager.FetchSearchResults(selectedItem);
                if (results.Count == 0)
                {
                    this.HideRescrape();
                    Toaster.Instance.ShowToast("Error fetching results", 0x9c4);
                }
                else
                {
                    this.lvScrape.ItemsSource = results;
                    this.lvScrape.Items.SortDescriptions.Clear();
                    this.lvScrape.Items.SortDescriptions.Add(new SortDescription("Rating", ListSortDirection.Ascending));
                    Console.WriteLine(this.lvScrape.Items.Count);
                    await this.SlideOut();
                    Toaster.Instance.HideToast();
                }
            }
        }

        private async void ShowWideScreenResults()
        {
            Game selectedItem = (Game) this.lvGames.SelectedItem;
            if (selectedItem != null)
            {
                this.rtbResults.Document.Blocks.Clear();
                this.lvGames.IsHitTestVisible = false;
                Toaster.Instance.ShowToast("Fetching wide screen patches");
                string src = await GameManager.FetchWideScreenPatches(selectedItem);
                if (src.IsEmpty())
                {
                    Toaster.Instance.ShowToast("No patches found", 0x5dc);
                    this.lvGames.IsHitTestVisible = true;
                }
                else
                {
                    this.rtbResults.AppendText(src);
                    this.gWideScreenResults.Visibility = Visibility.Visible;
                    this.gWideScreenResults.IsHitTestVisible = true;
                    this.lvGames.UnselectAll();
                    await this.SlideOut();
                    Toaster.Instance.HideToast();
                }
            }
        }

        private async Task SlideIn()
        {
            this.lvGames.Visibility = Visibility.Visible;
            this.lvGames.IsHitTestVisible = true;
            ThicknessAnimation a = new ThicknessAnimation {
                From = new Thickness(0.0, this.Height, 0.0, 0.0),
                To = 0.0,
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, this.lvGames);
            Storyboard.SetTargetProperty(a, new PropertyPath(FrameworkElement.MarginProperty));
            await storyboard.BeginAsync();
        }

        private async Task SlideOut()
        {
            this.lvGames.IsHitTestVisible = false;
            ThicknessAnimation a = new ThicknessAnimation {
                From = 0.0,
                To = new Thickness(0.0, this.Height, 0.0, 0.0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(a);
            Storyboard.SetTarget(a, this.lvGames);
            Storyboard.SetTargetProperty(a, new PropertyPath(FrameworkElement.MarginProperty));
            await storyboard.BeginAsync();
            this.lvGames.Visibility = Visibility.Collapsed;
        }

        private void SwitchView(string view)
        {
            if (this.lvGames.View == null)
            {
                ViewBase base2 = System.Windows.Application.Current.Resources["gridView"] as ViewBase;
                this.lvGames.View = base2;
            }
            if (view != "tv")
            {
                ViewBase base3 = System.Windows.Application.Current.Resources[view] as ViewBase;
                this.lvGames.View = base3;
            }
            if (view == "gridView")
            {
                Style style = base.Resources["lvDarkItemStyleLocal"] as Style;
                this.lvGames.ItemContainerStyle = style;
            }
            else if (view == "tileView")
            {
                Style style2 = base.Resources["lvGenericItemStyleLocal"] as Style;
                this.lvGames.ItemContainerStyle = style2;
            }
            else if (view == "tv")
            {
                base.Hide();
                this.btnTV.IsChecked = false;
                wndFullScreen screen = new wndFullScreen();
                if (screen.ShowDialog() == true)
                {
                    base.Show();
                }
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    ((MainWindow) target).Closing += new CancelEventHandler(this.Window_Closing);
                    return;

                case 4:
                    this.miAddFromImage = (System.Windows.Controls.MenuItem) target;
                    this.miAddFromImage.Click += new RoutedEventHandler(this.miAddFromImage_Click);
                    return;

                case 5:
                    this.tbSearch = (System.Windows.Controls.TextBox) target;
                    return;

                case 6:
                    this.gWideScreenResults = (Grid) target;
                    return;

                case 7:
                    this.btnSaveWidePatch = (System.Windows.Controls.Button) target;
                    return;

                case 8:
                    this.rtbResults = (System.Windows.Controls.RichTextBox) target;
                    return;

                case 9:
                    this.lvScrape = (System.Windows.Controls.ListView) target;
                    return;

                case 10:
                    this.lvSaveStates = (System.Windows.Controls.ListView) target;
                    return;

                case 11:
                    this.miRemoveStates = (System.Windows.Controls.MenuItem) target;
                    return;

                case 12:
                    this.btnStacked = (ImageButton) target;
                    return;

                case 13:
                    this.btnTile = (ImageButton) target;
                    return;

                case 14:
                    this.btnTV = (ImageButton) target;
                    return;

                case 15:
                    this.lvGames = (System.Windows.Controls.ListView) target;
                    return;

                case 0x10:
                    this.tbInfo = (TextBlock) target;
                    return;

                case 0x11:
                    this.imgPreview = (Image) target;
                    return;

                case 0x12:
                    this.tbDebug = (System.Windows.Controls.TextBox) target;
                    return;
            }
            this._contentLoaded = true;
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            EventSetter setter;
            switch (connectionId)
            {
                case 2:
                    setter = new EventSetter {
                        Event = System.Windows.Controls.Control.MouseDoubleClickEvent,
                        Handler = new MouseButtonEventHandler(this.lviDoubleClick)
                    };
                    ((Style) target).Setters.Add(setter);
                    return;

                case 3:
                    setter = new EventSetter {
                        Event = System.Windows.Controls.Control.MouseDoubleClickEvent,
                        Handler = new MouseButtonEventHandler(this.lviDoubleClick)
                    };
                    ((Style) target).Setters.Add(setter);
                    return;
            }
        }

        private void tbDebug_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Func<string, string, bool> func = (s, ss) => s.Equals(ss, StringComparison.InvariantCultureIgnoreCase);
                string str = this.tbDebug.Text.Trim();
                if (func(str, "add dummies"))
                {
                    this.AddDummies();
                }
                else if (func(str, "clear library"))
                {
                    List<Game> list = new List<Game>();
                    foreach (Game game in Game.AllGames)
                    {
                        list.Add(game);
                    }
                    foreach (Game game2 in list)
                    {
                        GameManager.Remove(game2);
                    }
                }
                else if (func(str, "crash"))
                {
                    this.crashMe();
                }
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            this._t.Stop();
            this._t.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (UserSettings.xGames != null)
            {
                UserSettings.xGames.Save(UserSettings.BonusXml);
            }
            this.SaveWindowSettings();
            System.Windows.Application.Current.Shutdown();
        }













    }
}

