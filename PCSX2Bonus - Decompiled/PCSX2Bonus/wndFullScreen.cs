namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    public sealed class wndFullScreen : Window, IComponentConnector, INotifyPropertyChanged, IStyleConnector
    {
        private Game _activeGame;
        private bool _contentLoaded;
        private ScrollViewer _sv;
        private DispatcherTimer _timer = new DispatcherTimer();
        private Gamepad gamePad;
        private bool gameSelected;
        internal Border infoPanel;
        private bool infoVisible;
        internal Grid itemHost;
        internal ListBox lbGames;
        private int selectedIndex;
        internal ScrollViewer svDescription;
        internal TextBlock tbLoading;

        public event PropertyChangedEventHandler PropertyChanged;

        public wndFullScreen()
        {
            this.InitializeComponent();
            base.Loaded += new RoutedEventHandler(this.wndFullScreen_Loaded);
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            Point position = Mouse.GetPosition(this.lbGames);
            if (position.Y > (this._sv.ViewportHeight - 20.0))
            {
                this._sv.LineDown();
            }
            else if (position.Y < 20.0)
            {
                this._sv.LineUp();
            }
        }

        private void AquireGamepad()
        {
            if (RuntimePolicyHelper.LegacyV2RuntimeEnabledSuccessfully)
            {
                this.gamePad = new Gamepad(this);
                if (this.gamePad.IsValid)
                {
                    this.gamePad.ButtonPressed += new EventHandler(this.gamePad_ButtonPressed);
                    this.gamePad.DirectionChanged += new EventHandler(this.gamePad_DirectionChanged);
                    this.gamePad.PollAsync();
                }
            }
        }

        private void btnInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.ShowInfo();
            }
        }

        private void btnPlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.LaunchGame();
            }
        }

        private void DisposeImage(BitmapImage img)
        {
            if (img != null)
            {
                try
                {
                    byte[] buffer = new byte[1];
                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        img.UriSource = null;
                        img.StreamSource = stream;
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }

        private void gamePad_ButtonPressed(object sender, EventArgs e)
        {
            Action callback = null;
            Action action2 = null;
            if (this.ActiveGame != null)
            {
                int num = (int) sender;
                if (num == Settings.Default.buttonOk)
                {
                    if (callback == null)
                    {
                        callback = delegate {
                            ListBoxItem element = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromItem(this.ActiveGame);
                            Image descendantByName = (Image) Tools.GetDescendantByName(element, "btnPlay");
                            Image image2 = (Image) Tools.GetDescendantByName(element, "btnInfo");
                            if (!this.gameSelected)
                            {
                                this.gameSelected = true;
                                element.Tag = "info";
                                descendantByName.Tag = "null";
                                image2.Tag = "null";
                            }
                            else if (descendantByName.Tag.ToString() == "selected")
                            {
                                this.LaunchGame();
                            }
                            else if (image2.Tag.ToString() == "selected")
                            {
                                this.ShowInfo();
                            }
                        };
                    }
                    base.Dispatcher.Invoke(callback);
                }
                else if (num == Settings.Default.buttonCancel)
                {
                    if (action2 == null)
                    {
                        action2 = delegate {
                            ListBoxItem element = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromItem(this.ActiveGame);
                            Image descendantByName = (Image) Tools.GetDescendantByName(element, "btnPlay");
                            Image image2 = (Image) Tools.GetDescendantByName(element, "btnInfo");
                            if (this.gameSelected && !this.infoVisible)
                            {
                                descendantByName.RaiseEvent(this.MouseLeaveArgs());
                                image2.RaiseEvent(this.MouseLeaveArgs());
                                this.gameSelected = false;
                                element.Tag = "null";
                            }
                            else if (this.gameSelected && this.infoVisible)
                            {
                                this.HideInfo();
                            }
                        };
                    }
                    base.Dispatcher.Invoke(action2);
                }
            }
        }

        private void gamePad_DirectionChanged(object sender, EventArgs e)
        {
            string dir = sender.ToString();
            base.Dispatcher.Invoke(delegate {
                if (this.gameSelected)
                {
                    ListBoxItem element = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromItem(this.ActiveGame);
                    Image descendantByName = (Image) Tools.GetDescendantByName(element, "btnPlay");
                    Image image2 = (Image) Tools.GetDescendantByName(element, "btnInfo");
                    if ((dir == "left") && !this.infoVisible)
                    {
                        descendantByName.Tag = "selected";
                        image2.Tag = "null";
                        descendantByName.RaiseEvent(this.MouseEnterArgs());
                        image2.RaiseEvent(this.MouseLeaveArgs());
                    }
                    else if ((dir == "right") && !this.infoVisible)
                    {
                        image2.Tag = "selected";
                        descendantByName.Tag = "null";
                        image2.RaiseEvent(this.MouseEnterArgs());
                        descendantByName.RaiseEvent(this.MouseLeaveArgs());
                    }
                    if (this.infoVisible)
                    {
                        if (dir == "up")
                        {
                            this.svDescription.LineUp();
                        }
                        else if (dir == "down")
                        {
                            this.svDescription.LineDown();
                        }
                    }
                }
                else
                {
                    switch (dir)
                    {
                        case "left":
                            this.NavigateItems(FocusNavigationDirection.Left);
                            break;

                        case "right":
                            this.NavigateItems(FocusNavigationDirection.Right);
                            break;

                        case "up":
                            this.NavigateItems(FocusNavigationDirection.Up);
                            break;

                        case "down":
                            this.NavigateItems(FocusNavigationDirection.Down);
                            break;
                    }
                }
            });
        }

        private void HideInfo()
        {
            this.infoVisible = false;
            this.infoPanel.Visibility = Visibility.Collapsed;
            this.lbGames.IsHitTestVisible = true;
        }

        private void infoPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                this.HideInfo();
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndfullscreen.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void itemHost_PreviewMouseMove(object sender, MouseEventArgs e)
        {
        }

        private void LaunchGame()
        {
            if (this.ActiveGame != null)
            {
                MainWindow mainWindow = (MainWindow) Application.Current.MainWindow;
                base.Hide();
                mainWindow.LaunchGame(this.ActiveGame, true);
            }
        }

        public void lbItemKeyDown(object sender, KeyEventArgs e)
        {
            ListBoxItem element = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromItem(this.ActiveGame);
            if (this.gameSelected)
            {
                if (e.Key == Key.Left)
                {
                    Button descendantByName = (Button) Tools.GetDescendantByName(element, "btnPlay");
                }
                else if (e.Key == Key.Right)
                {
                    Button button2 = (Button) Tools.GetDescendantByName(element, "btnInfo");
                }
            }
            else if (e.Key == Key.Return)
            {
                this.gameSelected = true;
                element.Tag = "info";
            }
        }

        public void lbItemMouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxItem item = (ListBoxItem) sender;
            Game dataContext = (Game) item.DataContext;
            if ((this.ActiveGame != null) && (this.ActiveGame != dataContext))
            {
                ListBoxItem item2 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromItem(this.ActiveGame);
                item2.Tag = "null";
                item2.RaiseEvent(this.MouseLeaveArgs());
                this.gameSelected = false;
            }
            this.ActiveGame = dataContext;
        }

        public MouseEventArgs MouseEnterArgs()
        {
            TimeSpan span = new TimeSpan(DateTime.Now.Ticks);
            return new MouseEventArgs(Mouse.PrimaryDevice, span.Milliseconds) { RoutedEvent = Mouse.MouseEnterEvent };
        }

        public MouseEventArgs MouseLeaveArgs()
        {
            TimeSpan span = new TimeSpan(DateTime.Now.Ticks);
            return new MouseEventArgs(Mouse.PrimaryDevice, span.Milliseconds) { RoutedEvent = Mouse.MouseLeaveEvent };
        }

        private void NavigateItems(FocusNavigationDirection dir)
        {
            TimeSpan span = new TimeSpan(DateTime.Now.Ticks);
            int milliseconds = span.Milliseconds;
            if (dir == FocusNavigationDirection.Right)
            {
                if ((this.selectedIndex + 1) < Game.AllGames.Count)
                {
                    this.selectedIndex++;
                }
                if ((this.selectedIndex + 1) > 0)
                {
                    ListBoxItem item = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex - 1);
                    MouseEventArgs args = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                        RoutedEvent = Mouse.MouseLeaveEvent
                    };
                    Panel.SetZIndex(item, 0);
                    item.RaiseEvent(args);
                    item.IsSelected = false;
                }
                ListBoxItem element = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex);
                Panel.SetZIndex(element, 10);
                MouseEventArgs e = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                    RoutedEvent = Mouse.MouseEnterEvent
                };
                element.RaiseEvent(e);
                element.Focus();
                this.lbGames.SelectedItem = this.lbGames.Items[this.selectedIndex];
                this.lbGames.ScrollIntoView(this.lbGames.Items[this.selectedIndex]);
                this.ActiveGame = (Game) this.lbGames.SelectedItem;
            }
            else if (dir == FocusNavigationDirection.Left)
            {
                if (this.selectedIndex > 0)
                {
                    this.selectedIndex--;
                }
                if ((this.selectedIndex + 1) < this.lbGames.Items.Count)
                {
                    ListBoxItem item3 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex + 1);
                    MouseEventArgs args5 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                        RoutedEvent = Mouse.MouseLeaveEvent
                    };
                    Panel.SetZIndex(item3, 0);
                    item3.RaiseEvent(args5);
                    item3.IsSelected = false;
                }
                ListBoxItem item4 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex);
                Panel.SetZIndex(item4, 10);
                MouseEventArgs args7 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                    RoutedEvent = Mouse.MouseEnterEvent
                };
                item4.RaiseEvent(args7);
                item4.Focus();
                this.lbGames.SelectedItem = this.lbGames.Items[this.selectedIndex];
                this.lbGames.ScrollIntoView(this.lbGames.Items[this.selectedIndex]);
                this.ActiveGame = (Game) this.lbGames.SelectedItem;
            }
            else if (dir == FocusNavigationDirection.Down)
            {
                ListBoxItem item5 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex);
                int num2 = ((int) Math.Floor((double) (this.lbGames.ActualWidth / item5.ActualWidth))) + this.selectedIndex;
                if ((num2 + 1) <= this.lbGames.Items.Count)
                {
                    if ((this.selectedIndex + 1) > 0)
                    {
                        MouseEventArgs args9 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                            RoutedEvent = Mouse.MouseLeaveEvent
                        };
                        Panel.SetZIndex(item5, 0);
                        item5.RaiseEvent(args9);
                        item5.IsSelected = false;
                    }
                    this.selectedIndex = num2;
                    item5 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex);
                    Panel.SetZIndex(item5, 10);
                    MouseEventArgs args11 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                        RoutedEvent = Mouse.MouseEnterEvent
                    };
                    item5.RaiseEvent(args11);
                    item5.Focus();
                    this.lbGames.SelectedItem = this.lbGames.Items[this.selectedIndex];
                    this.lbGames.ScrollIntoView(this.lbGames.Items[this.selectedIndex]);
                    this.ActiveGame = (Game) this.lbGames.SelectedItem;
                }
            }
            else if (dir == FocusNavigationDirection.Up)
            {
                ListBoxItem item6 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex);
                int num3 = this.selectedIndex - ((int) Math.Floor((double) (this.lbGames.ActualWidth / item6.ActualWidth)));
                if (num3 >= 0)
                {
                    if (this.selectedIndex > 0)
                    {
                        MouseEventArgs args13 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                            RoutedEvent = Mouse.MouseLeaveEvent
                        };
                        Panel.SetZIndex(item6, 0);
                        item6.RaiseEvent(args13);
                        item6.IsSelected = false;
                    }
                    this.selectedIndex = num3;
                    item6 = (ListBoxItem) this.lbGames.ItemContainerGenerator.ContainerFromIndex(this.selectedIndex);
                    Panel.SetZIndex(item6, 10);
                    MouseEventArgs args15 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
                        RoutedEvent = Mouse.MouseEnterEvent
                    };
                    item6.RaiseEvent(args15);
                    item6.Focus();
                    this.lbGames.SelectedItem = this.lbGames.Items[this.selectedIndex];
                    this.lbGames.ScrollIntoView(this.lbGames.Items[this.selectedIndex]);
                    this.ActiveGame = (Game) this.lbGames.SelectedItem;
                }
            }
        }

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private void ShowInfo()
        {
            if (this.ActiveGame != null)
            {
                this.svDescription.ScrollToVerticalOffset(0.0);
                this.infoVisible = true;
                this.infoPanel.Visibility = Visibility.Visible;
                this.lbGames.IsHitTestVisible = false;
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.itemHost = (Grid) target;
                    return;

                case 2:
                    this.lbGames = (ListBox) target;
                    return;

                case 6:
                    this.tbLoading = (TextBlock) target;
                    return;

                case 7:
                    this.infoPanel = (Border) target;
                    this.infoPanel.MouseDown += new MouseButtonEventHandler(this.infoPanel_MouseDown);
                    return;

                case 8:
                    this.svDescription = (ScrollViewer) target;
                    return;
            }
            this._contentLoaded = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 3:
                {
                    EventSetter item = new EventSetter {
                        Event = UIElement.MouseEnterEvent,
                        Handler = new MouseEventHandler(this.lbItemMouseEnter)
                    };
                    ((Style) target).Setters.Add(item);
                    item = new EventSetter {
                        Event = UIElement.KeyDownEvent,
                        Handler = new KeyEventHandler(this.lbItemKeyDown)
                    };
                    ((Style) target).Setters.Add(item);
                    return;
                }
                case 4:
                    ((Image) target).MouseDown += new MouseButtonEventHandler(this.btnPlay_MouseDown);
                    return;

                case 5:
                    ((Image) target).MouseDown += new MouseButtonEventHandler(this.btnInfo_MouseDown);
                    return;
            }
        }

        private void wndFullScreen_Closing(object sender, CancelEventArgs e)
        {
            base.DialogResult = true;
            if (this.gamePad != null)
            {
                this.gamePad.CancelPollAsync();
                this.gamePad.Dispose();
            }
            this._timer.Stop();
            this._timer = null;
            GC.Collect();
        }

        private void wndFullScreen_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.gamePad != null)
            {
                if (base.IsVisible)
                {
                    if (this.gamePad != null)
                    {
                        this.gamePad.CancelPollAsync();
                        this.gamePad.PollAsync();
                    }
                }
                else if (!base.IsVisible)
                {
                    this.gamePad.CancelPollAsync();
                }
            }
        }

        private void wndFullScreen_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.enableGamepad)
            {
                this.AquireGamepad();
            }
            string defaultSort = Settings.Default.defaultSort;
            if (defaultSort != null)
            {
                if (!(defaultSort == "Alphabetical"))
                {
                    if (defaultSort == "Serial")
                    {
                        this.lbGames.Items.SortDescriptions.Add(new SortDescription("Serial", ListSortDirection.Ascending));
                    }
                    else if (defaultSort == "Default")
                    {
                    }
                }
                else
                {
                    this.lbGames.Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
                }
            }
            this._sv = (ScrollViewer) Tools.GetDescendantByType(this.lbGames, typeof(ScrollViewer));
            this._timer.Interval = TimeSpan.FromMilliseconds(0.5);
            this._timer.Tick += new EventHandler(this._timer_Tick);
            this._timer.Start();
            base.PreviewKeyDown += new KeyEventHandler(this.wndFullScreen_PreviewKeyDown);
            base.Closing += new CancelEventHandler(this.wndFullScreen_Closing);
            base.IsVisibleChanged += new DependencyPropertyChangedEventHandler(this.wndFullScreen_IsVisibleChanged);
            base.PreviewMouseDown += new MouseButtonEventHandler(this.wndFullScreen_PreviewMouseDown);
            this.itemHost.PreviewMouseMove += new MouseEventHandler(this.itemHost_PreviewMouseMove);
        }

        private void wndFullScreen_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                base.Close();
            }
        }

        private void wndFullScreen_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                ScrollViewer descendantByType = (ScrollViewer) Tools.GetDescendantByType(this.lbGames, typeof(ScrollViewer));
                if (descendantByType != null)
                {
                    if (descendantByType.VerticalScrollBarVisibility == ScrollBarVisibility.Hidden)
                    {
                        descendantByType.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    }
                    else if (descendantByType.VerticalScrollBarVisibility == ScrollBarVisibility.Visible)
                    {
                        descendantByType.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    }
                }
            }
        }

        public Game ActiveGame
        {
            get
            {
                return this._activeGame;
            }
            set
            {
                this._activeGame = value;
                this.OnPropertyChanged("ActiveGame");
            }
        }
    }
}

