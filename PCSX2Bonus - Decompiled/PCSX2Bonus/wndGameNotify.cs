namespace PCSX2Bonus
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Markup;
    using System.Windows.Media.Animation;

    public sealed class wndGameNotify : Window, IComponentConnector
    {
        private bool _contentLoaded;

        public wndGameNotify()
        {
            this.InitializeComponent();
            base.Topmost = true;
            base.Opacity = 0.0;
            base.Loaded += new RoutedEventHandler(this.wndGameNotify_Loaded);
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndgamenotify.xaml", UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocator);
            }
        }

        private async void SlideIn()
        {
            this.Opacity = 1.0;
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            DoubleAnimation a_top = new DoubleAnimation {
                From = new double?(workingArea.Bottom + this.Height),
                To = new double?((workingArea.Bottom - this.Height) - 5.0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            DoubleAnimation a_bottom = new DoubleAnimation {
                To = new double?((workingArea.Bottom + this.Height) + 5.0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(a_top);
            Storyboard.SetTarget(a_top, this);
            Storyboard.SetTargetProperty(a_top, new PropertyPath(Window.TopProperty));
            await storyboard.BeginAsync();
            await Task.Delay(0xbb8);
            storyboard.Children.Clear();
            storyboard.Children.Add(a_bottom);
            Storyboard.SetTarget(a_bottom, this);
            Storyboard.SetTargetProperty(a_bottom, new PropertyPath(Window.TopProperty));
            await storyboard.BeginAsync();
            this.Close();
        }

        [EditorBrowsable(EditorBrowsableState.Never), GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            this._contentLoaded = true;
        }

        private void wndGameNotify_Loaded(object sender, RoutedEventArgs e)
        {
            Rectangle workingArea = Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
            base.Left = (workingArea.Right - base.Width) - 5.0;
            this.SlideIn();
        }

        private void wndGameNotify_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            base.Close();
        }

    }
}

