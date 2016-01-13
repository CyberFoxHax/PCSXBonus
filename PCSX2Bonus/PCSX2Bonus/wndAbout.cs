namespace PCSX2Bonus
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public sealed class wndAbout : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal Button btnOk;
        internal StackPanel spThanks;

        public wndAbout()
        {
            this.InitializeComponent();
            base.Owner = Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndAbout_Loaded);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndabout.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.spThanks = (StackPanel) target;
                    return;

                case 2:
                    this.btnOk = (Button) target;
                    this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndAbout_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

