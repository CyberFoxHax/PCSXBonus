namespace PCSX2Bonus {
	using Properties;
	using System;
	using System.CodeDom.Compiler;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Input;
	using System.Windows.Markup;
	using System.Windows.Media.Imaging;
	using System.Windows.Threading;

	public sealed class wndFullScreen : Window, IComponentConnector, INotifyPropertyChanged, IStyleConnector {
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

		public wndFullScreen() {
			InitializeComponent();
			base.Loaded += wndFullScreen_Loaded;
		}

		private void _timer_Tick(object sender, EventArgs e) {
			var position = Mouse.GetPosition(lbGames);
			if (position.Y > (_sv.ViewportHeight - 20.0)) {
				_sv.LineDown();
			}
			else if (position.Y < 20.0) {
				_sv.LineUp();
			}
		}

		private void AquireGamepad() {
			gamePad = new Gamepad(this);
			if (gamePad.IsValid) {
				gamePad.ButtonPressed += gamePad_ButtonPressed;
				gamePad.DirectionChanged += gamePad_DirectionChanged;
				gamePad.PollAsync();
			}
		}

		private void btnInfo_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton == MouseButtonState.Pressed) {
				ShowInfo();
			}
		}

		private void btnPlay_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton == MouseButtonState.Pressed) {
				LaunchGame();
			}
		}

		private void DisposeImage(BitmapImage img) {
			if (img != null) {
				try {
					var buffer = new byte[1];
					using (var stream = new MemoryStream(buffer)) {
						img.UriSource = null;
						img.StreamSource = stream;
					}
				}
				catch (Exception exception) {
					Console.WriteLine(exception.Message);
				}
			}
		}

		private void gamePad_ButtonPressed(object sender, EventArgs e) {
			Action callback = null;
			Action action2 = null;
			if (ActiveGame != null) {
				var num = (int)sender;
				if (num == Settings.Default.buttonOk) {
					if (callback == null) {
						callback = delegate {
							var element = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromItem(ActiveGame);
							var descendantByName = (Image)Tools.GetDescendantByName(element, "btnPlay");
							var image2 = (Image)Tools.GetDescendantByName(element, "btnInfo");
							if (!gameSelected) {
								gameSelected = true;
								element.Tag = "info";
								descendantByName.Tag = "null";
								image2.Tag = "null";
							}
							else if (descendantByName.Tag.ToString() == "selected") {
								LaunchGame();
							}
							else if (image2.Tag.ToString() == "selected") {
								ShowInfo();
							}
						};
					}
					base.Dispatcher.Invoke(callback);
				}
				else if (num == Settings.Default.buttonCancel) {
					if (action2 == null) {
						action2 = delegate {
							var element = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromItem(ActiveGame);
							var descendantByName = (Image)Tools.GetDescendantByName(element, "btnPlay");
							var image2 = (Image)Tools.GetDescendantByName(element, "btnInfo");
							if (gameSelected && !infoVisible) {
								descendantByName.RaiseEvent(MouseLeaveArgs());
								image2.RaiseEvent(MouseLeaveArgs());
								gameSelected = false;
								element.Tag = "null";
							}
							else if (gameSelected && infoVisible) {
								HideInfo();
							}
						};
					}
					base.Dispatcher.Invoke(action2);
				}
			}
		}

		private void gamePad_DirectionChanged(object sender, EventArgs e) {
			var dir = sender.ToString();
			base.Dispatcher.Invoke(delegate {
				if (gameSelected) {
					var element = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromItem(ActiveGame);
					var descendantByName = (Image)Tools.GetDescendantByName(element, "btnPlay");
					var image2 = (Image)Tools.GetDescendantByName(element, "btnInfo");
					if ((dir == "left") && !infoVisible) {
						descendantByName.Tag = "selected";
						image2.Tag = "null";
						descendantByName.RaiseEvent(MouseEnterArgs());
						image2.RaiseEvent(MouseLeaveArgs());
					}
					else if ((dir == "right") && !infoVisible) {
						image2.Tag = "selected";
						descendantByName.Tag = "null";
						image2.RaiseEvent(MouseEnterArgs());
						descendantByName.RaiseEvent(MouseLeaveArgs());
					}
					if (infoVisible) {
						if (dir == "up") {
							svDescription.LineUp();
						}
						else if (dir == "down") {
							svDescription.LineDown();
						}
					}
				}
				else {
					switch (dir) {
						case "left":
							NavigateItems(FocusNavigationDirection.Left);
							break;

						case "right":
							NavigateItems(FocusNavigationDirection.Right);
							break;

						case "up":
							NavigateItems(FocusNavigationDirection.Up);
							break;

						case "down":
							NavigateItems(FocusNavigationDirection.Down);
							break;
					}
				}
			});
		}

		private void HideInfo() {
			infoVisible = false;
			infoPanel.Visibility = Visibility.Collapsed;
			lbGames.IsHitTestVisible = true;
		}

		private void infoPanel_MouseDown(object sender, MouseButtonEventArgs e) {
			if (e.RightButton == MouseButtonState.Pressed) {
				HideInfo();
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent() {
			if (!_contentLoaded) {
				_contentLoaded = true;
				var resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndfullscreen.xaml", UriKind.Relative);
				Application.LoadComponent(this, resourceLocator);
			}
		}

		private void itemHost_PreviewMouseMove(object sender, MouseEventArgs e) {
		}

		private void LaunchGame() {
			if (ActiveGame != null) {
				var mainWindow = (MainWindow)Application.Current.MainWindow;
				base.Hide();
				mainWindow.LaunchGame(ActiveGame, true);
			}
		}

		public void lbItemKeyDown(object sender, KeyEventArgs e) {
			var element = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromItem(ActiveGame);
			if (gameSelected) {
				if (e.Key == Key.Left) {
					var descendantByName = (Button)Tools.GetDescendantByName(element, "btnPlay");
				}
				else if (e.Key == Key.Right) {
					var button2 = (Button)Tools.GetDescendantByName(element, "btnInfo");
				}
			}
			else if (e.Key == Key.Return) {
				gameSelected = true;
				element.Tag = "info";
			}
		}

		public void lbItemMouseEnter(object sender, MouseEventArgs e) {
			var item = (ListBoxItem)sender;
			var dataContext = (Game)item.DataContext;
			if ((ActiveGame != null) && (ActiveGame != dataContext)) {
				var item2 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromItem(ActiveGame);
				item2.Tag = "null";
				item2.RaiseEvent(MouseLeaveArgs());
				gameSelected = false;
			}
			ActiveGame = dataContext;
		}

		public MouseEventArgs MouseEnterArgs() {
			var span = new TimeSpan(DateTime.Now.Ticks);
			return new MouseEventArgs(Mouse.PrimaryDevice, span.Milliseconds) { RoutedEvent = Mouse.MouseEnterEvent };
		}

		public MouseEventArgs MouseLeaveArgs() {
			var span = new TimeSpan(DateTime.Now.Ticks);
			return new MouseEventArgs(Mouse.PrimaryDevice, span.Milliseconds) { RoutedEvent = Mouse.MouseLeaveEvent };
		}

		private void NavigateItems(FocusNavigationDirection dir) {
			var span = new TimeSpan(DateTime.Now.Ticks);
			var milliseconds = span.Milliseconds;
			if (dir == FocusNavigationDirection.Right) {
				if ((selectedIndex + 1) < Game.AllGames.Count) {
					selectedIndex++;
				}
				if ((selectedIndex + 1) > 0) {
					var item = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex - 1);
					var args = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
						RoutedEvent = Mouse.MouseLeaveEvent
					};
					Panel.SetZIndex(item, 0);
					item.RaiseEvent(args);
					item.IsSelected = false;
				}
				var element = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
				Panel.SetZIndex(element, 10);
				var e = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
					RoutedEvent = Mouse.MouseEnterEvent
				};
				element.RaiseEvent(e);
				element.Focus();
				lbGames.SelectedItem = lbGames.Items[selectedIndex];
				lbGames.ScrollIntoView(lbGames.Items[selectedIndex]);
				ActiveGame = (Game)lbGames.SelectedItem;
			}
			else if (dir == FocusNavigationDirection.Left) {
				if (selectedIndex > 0) {
					selectedIndex--;
				}
				if ((selectedIndex + 1) < lbGames.Items.Count) {
					var item3 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex + 1);
					var args5 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
						RoutedEvent = Mouse.MouseLeaveEvent
					};
					Panel.SetZIndex(item3, 0);
					item3.RaiseEvent(args5);
					item3.IsSelected = false;
				}
				var item4 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
				Panel.SetZIndex(item4, 10);
				var args7 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
					RoutedEvent = Mouse.MouseEnterEvent
				};
				item4.RaiseEvent(args7);
				item4.Focus();
				lbGames.SelectedItem = lbGames.Items[selectedIndex];
				lbGames.ScrollIntoView(lbGames.Items[selectedIndex]);
				ActiveGame = (Game)lbGames.SelectedItem;
			}
			else if (dir == FocusNavigationDirection.Down) {
				var item5 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
				var num2 = ((int)Math.Floor((double)(lbGames.ActualWidth / item5.ActualWidth))) + selectedIndex;
				if ((num2 + 1) <= lbGames.Items.Count) {
					if ((selectedIndex + 1) > 0) {
						var args9 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
							RoutedEvent = Mouse.MouseLeaveEvent
						};
						Panel.SetZIndex(item5, 0);
						item5.RaiseEvent(args9);
						item5.IsSelected = false;
					}
					selectedIndex = num2;
					item5 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
					Panel.SetZIndex(item5, 10);
					var args11 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
						RoutedEvent = Mouse.MouseEnterEvent
					};
					item5.RaiseEvent(args11);
					item5.Focus();
					lbGames.SelectedItem = lbGames.Items[selectedIndex];
					lbGames.ScrollIntoView(lbGames.Items[selectedIndex]);
					ActiveGame = (Game)lbGames.SelectedItem;
				}
			}
			else if (dir == FocusNavigationDirection.Up) {
				var item6 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
				var num3 = selectedIndex - ((int)Math.Floor((double)(lbGames.ActualWidth / item6.ActualWidth)));
				if (num3 >= 0) {
					if (selectedIndex > 0) {
						var args13 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
							RoutedEvent = Mouse.MouseLeaveEvent
						};
						Panel.SetZIndex(item6, 0);
						item6.RaiseEvent(args13);
						item6.IsSelected = false;
					}
					selectedIndex = num3;
					item6 = (ListBoxItem)lbGames.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
					Panel.SetZIndex(item6, 10);
					var args15 = new MouseEventArgs(Mouse.PrimaryDevice, milliseconds) {
						RoutedEvent = Mouse.MouseEnterEvent
					};
					item6.RaiseEvent(args15);
					item6.Focus();
					lbGames.SelectedItem = lbGames.Items[selectedIndex];
					lbGames.ScrollIntoView(lbGames.Items[selectedIndex]);
					ActiveGame = (Game)lbGames.SelectedItem;
				}
			}
		}

		private void OnPropertyChanged(string property) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(property));
			}
		}

		private void ShowInfo() {
			if (ActiveGame != null) {
				svDescription.ScrollToVerticalOffset(0.0);
				infoVisible = true;
				infoPanel.Visibility = Visibility.Visible;
				lbGames.IsHitTestVisible = false;
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 1:
					itemHost = (Grid)target;
					return;

				case 2:
					lbGames = (ListBox)target;
					return;

				case 6:
					tbLoading = (TextBlock)target;
					return;

				case 7:
					infoPanel = (Border)target;
					infoPanel.MouseDown += infoPanel_MouseDown;
					return;

				case 8:
					svDescription = (ScrollViewer)target;
					return;
			}
			_contentLoaded = true;
		}

		[EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		void IStyleConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 3: {
						var item = new EventSetter {
							Event = MouseEnterEvent,
							Handler = new MouseEventHandler(lbItemMouseEnter)
						};
						((Style)target).Setters.Add(item);
						item = new EventSetter {
							Event = KeyDownEvent,
							Handler = new KeyEventHandler(lbItemKeyDown)
						};
						((Style)target).Setters.Add(item);
						return;
					}
				case 4:
					((Image)target).MouseDown += btnPlay_MouseDown;
					return;

				case 5:
					((Image)target).MouseDown += btnInfo_MouseDown;
					return;
			}
		}

		private void wndFullScreen_Closing(object sender, CancelEventArgs e) {
			base.DialogResult = true;
			if (gamePad != null) {
				gamePad.CancelPollAsync();
				gamePad.Dispose();
			}
			_timer.Stop();
			_timer = null;
			GC.Collect();
		}

		private void wndFullScreen_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if (gamePad == null) return;
			if (IsVisible) {
				if (gamePad == null) return;
				gamePad.CancelPollAsync();
				gamePad.PollAsync();
			}
			else {
				gamePad.CancelPollAsync();
			}
		}

		private void wndFullScreen_Loaded(object sender, RoutedEventArgs e) {
			if (Settings.Default.enableGamepad) {
				AquireGamepad();
			}
			var defaultSort = Settings.Default.defaultSort;
			if (defaultSort != null) {
				if (defaultSort != "Alphabetical") {
					switch (defaultSort) {
						case "Serial":
							lbGames.Items.SortDescriptions.Add(new SortDescription("Serial", ListSortDirection.Ascending));
							break;
						case "Default":
							break;
					}
				}
				else {
					lbGames.Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
				}
			}
			_sv = (ScrollViewer)Tools.GetDescendantByType(lbGames, typeof(ScrollViewer));
			_timer.Interval = TimeSpan.FromMilliseconds(0.5);
			_timer.Tick += _timer_Tick;
			_timer.Start();
			PreviewKeyDown += wndFullScreen_PreviewKeyDown;
			Closing += wndFullScreen_Closing;
			IsVisibleChanged += wndFullScreen_IsVisibleChanged;
			PreviewMouseDown += wndFullScreen_PreviewMouseDown;
			itemHost.PreviewMouseMove += itemHost_PreviewMouseMove;
		}

		private void wndFullScreen_PreviewKeyDown(object sender, KeyEventArgs e) {
			if (e.Key == Key.Escape)
				Close();
		}

		private void wndFullScreen_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if (e.RightButton != MouseButtonState.Pressed) return;
			var descendantByType = (ScrollViewer)Tools.GetDescendantByType(lbGames, typeof(ScrollViewer));
			if (descendantByType == null) return;
			switch (descendantByType.VerticalScrollBarVisibility) {
				case ScrollBarVisibility.Hidden:
					descendantByType.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
					break;
				case ScrollBarVisibility.Visible:
					descendantByType.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
					break;
			}
		}

		public Game ActiveGame {
			get {
				return _activeGame;
			}
			set {
				_activeGame = value;
				OnPropertyChanged("ActiveGame");
			}
		}
	}
}

