namespace PCSX2Bonus
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media.Imaging;

    public sealed class wndScreenshots : Window, IComponentConnector, IStyleConnector
    {
        private bool _contentLoaded;
        private ObservableCollection<BitmapImage> _imageLinks = new ObservableCollection<BitmapImage>();
        internal Grid bottomGrid;
        internal Button btnSave;
        private CancellationTokenSource cts = new CancellationTokenSource();
        private Game g;
        internal Image imgBig;
        internal TextBlock tbInfo;
        internal Grid topGrid;

        public wndScreenshots()
        {
            this.InitializeComponent();
            base.Owner = Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndScreenshots_Loaded);
            base.Closing += new CancelEventHandler(this.wndScreenshots_Closing);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Predicate<BitmapImage> match = null;
            try
            {
                if (!Directory.Exists(Path.Combine(UserSettings.ScreensDir, this.g.FileSafeTitle)))
                {
                    Directory.CreateDirectory(Path.Combine(UserSettings.ScreensDir, this.g.FileSafeTitle));
                }
                if (match == null)
                {
                    match = i => i == ((BitmapImage) this.imgBig.Source);
                }
                int num = Array.FindIndex<BitmapImage>(this._imageLinks.ToArray<BitmapImage>(), match);
                if (num == -1)
                {
                    throw new Exception("index out of bounds");
                }
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                string path = Path.Combine(Path.Combine(UserSettings.ScreensDir, this.g.FileSafeTitle), string.Concat(new object[] { this.g.FileSafeTitle, "-", num, ".jpg" }));
                encoder.Frames.Add(BitmapFrame.Create((BitmapImage) this.imgBig.Source));
                using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    encoder.Save(stream);
                }
                Toaster.Instance.ShowToast("Successfully saved screenshot to " + path, 0x5dc);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Toaster.Instance.ShowToast("Error saving screenshot", 0x5dc);
            }
        }

        public void ImageClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.imgBig.Source = ((Image) sender).Source;
                this.imgBig.Visibility = Visibility.Visible;
            }
        }

        private void imgBig_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.imgBig.Visibility = Visibility.Collapsed;
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndscreenshots.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 2:
                    this.btnSave = (Button) target;
                    this.btnSave.Click += new RoutedEventHandler(this.btnSave_Click);
                    return;

                case 3:
                    this.topGrid = (Grid) target;
                    return;

                case 4:
                    this.bottomGrid = (Grid) target;
                    return;

                case 5:
                    this.imgBig = (Image) target;
                    return;

                case 6:
                    this.tbInfo = (TextBlock) target;
                    return;
            }
            this._contentLoaded = true;
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
        void IStyleConnector.Connect(int connectionId, object target)
        {
            if (connectionId == 1)
            {
                EventSetter item = new EventSetter {
                    Event = UIElement.MouseDownEvent,
                    Handler = new MouseButtonEventHandler(this.ImageClicked)
                };
                ((Style) target).Setters.Add(item);
            }
        }

        private void wndScreenshots_Closing(object sender, CancelEventArgs e)
        {
            this.cts.Cancel();
            Toaster.Instance.HideToast();
            base.DialogResult = true;
        }

        private async void wndScreenshots_Loaded(object sender, RoutedEventArgs e)
        {
            this.g = (Game) this.Tag;
            this.Title = "Viewing screenshots for " + ((Game) this.Tag).Title;
            this.imgBig.MouseDown += new MouseButtonEventHandler(this.imgBig_MouseDown);
            Toaster.Instance.ShowToast("Loading screenshots");
            List<string> imageLinks = await GameManager.FetchScreenshots((Game) this.Tag);
            if (imageLinks.Count == 0)
            {
                this.Close();
            }
            else
            {
                try
                {
                    List<string>.Enumerator enumerator = imageLinks.GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            string current = enumerator.Current;
                            if (File.Exists(current))
                            {
                                using (FileStream stream = File.OpenRead(current))
                                {
                                    BitmapImage item = new BitmapImage();
                                    item.BeginInit();
                                    item.StreamSource = stream;
                                    item.CacheOption = BitmapCacheOption.OnLoad;
                                    item.EndInit();
                                    this._imageLinks.Add(item);
                                    continue;
                                }
                            }
                            BitmapImage bmi = await Tools.ImageFromWeb(current, this.cts.Token);
                            this._imageLinks.Add(bmi);
                        }
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }
                }
                catch
                {
                }
                Toaster.Instance.HideToast();
            }
        }

        public ObservableCollection<BitmapImage> ImageLinks
        {
            get
            {
                return this._imageLinks;
            }
        }

    }
}

