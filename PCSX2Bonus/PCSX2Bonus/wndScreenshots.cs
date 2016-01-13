using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace PCSX2Bonus.PCSX2Bonus {
	public sealed class wndScreenshots : Window, IComponentConnector, IStyleConnector {
		private bool _contentLoaded;
		private readonly ObservableCollection<BitmapImage> _imageLinks = new ObservableCollection<BitmapImage>();
		internal Grid bottomGrid;
		internal Button btnSave;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private Game g;
		internal Image imgBig;
		internal TextBlock tbInfo;
		internal Grid topGrid;

		public wndScreenshots() {
			InitializeComponent();
			Owner = Application.Current.MainWindow;
			Loaded += wndScreenshots_Loaded;
			Closing += wndScreenshots_Closing;
		}

		private void btnSave_Click(object sender, RoutedEventArgs e) {
			try {
				if (!Directory.Exists(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle)))
					Directory.CreateDirectory(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle));
				Predicate<BitmapImage> match = i => i == ((BitmapImage)imgBig.Source);
				var num = Array.FindIndex<BitmapImage>(_imageLinks.ToArray<BitmapImage>(), match);
				if (num == -1)
					throw new Exception("index out of bounds");
				var encoder = new JpegBitmapEncoder();
				var path = Path.Combine(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle), string.Concat(new object[] { g.FileSafeTitle, "-", num, ".jpg" }));
				encoder.Frames.Add(BitmapFrame.Create((BitmapImage)imgBig.Source));
				using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
					encoder.Save(stream);
				Toaster.Instance.ShowToast("Successfully saved screenshot to " + path, 0x5dc);
			}
			catch (Exception exception) {
				Console.WriteLine(exception.Message);
				Toaster.Instance.ShowToast("Error saving screenshot", 0x5dc);
			}
		}

		public void ImageClicked(object sender, MouseButtonEventArgs e) {
			if (e.LeftButton != MouseButtonState.Pressed) return;
			imgBig.Source = ((Image)sender).Source;
			imgBig.Visibility = Visibility.Visible;
		}

		private void imgBig_MouseDown(object sender, MouseButtonEventArgs e) {
			imgBig.Visibility = Visibility.Collapsed;
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent() {
			if (_contentLoaded) return;
			_contentLoaded = true;
			var resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndscreenshots.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 2:
					btnSave = (Button)target;
					btnSave.Click += btnSave_Click;
					return;

				case 3:
					topGrid = (Grid)target;
					return;

				case 4:
					bottomGrid = (Grid)target;
					return;

				case 5:
					imgBig = (Image)target;
					return;

				case 6:
					tbInfo = (TextBlock)target;
					return;
			}
			_contentLoaded = true;
		}

		[DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target) {
			if (connectionId == 1) {
				var item = new EventSetter {
					Event = MouseDownEvent,
					Handler = new MouseButtonEventHandler(ImageClicked)
				};
				((Style)target).Setters.Add(item);
			}
		}

		private void wndScreenshots_Closing(object sender, CancelEventArgs e) {
			_cts.Cancel();
			Toaster.Instance.HideToast();
			base.DialogResult = true;
		}

		private async void wndScreenshots_Loaded(object sender, RoutedEventArgs e) {
			g = (Game)Tag;
			Title = "Viewing screenshots for " + ((Game)Tag).Title;
			imgBig.MouseDown += imgBig_MouseDown;
			Toaster.Instance.ShowToast("Loading screenshots");
			List<string> imageLinks = await GameManager.FetchScreenshots((Game)Tag);
			if (imageLinks.Count == 0)
				Close();
			else {
				var enumerator = imageLinks.GetEnumerator();
				try {
					while (enumerator.MoveNext()) {
						var current = enumerator.Current;
						if (File.Exists(current)) {
							using (var stream = File.OpenRead(current)) {
								var item = new BitmapImage();
								item.BeginInit();
								item.StreamSource = stream;
								item.CacheOption = BitmapCacheOption.OnLoad;
								item.EndInit();
								_imageLinks.Add(item);
								continue;
							}
						}
						var bmi = await Tools.ImageFromWeb(current, _cts.Token);
						_imageLinks.Add(bmi);
					}
				}
				finally {
					enumerator.Dispose();
				}
				Toaster.Instance.HideToast();
			}
		}

		public ObservableCollection<BitmapImage> ImageLinks {
			get {
				return _imageLinks;
			}
		}

	}
}

