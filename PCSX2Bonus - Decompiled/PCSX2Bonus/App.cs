namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using SmartAssembly.MemoryManagement;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;

    public sealed class App : Application
    {
        private bool _contentLoaded;

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                base.StartupUri = new Uri("GUI/MainWindow.xaml", UriKind.Relative);
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/app.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), STAThread, DebuggerNonUserCode]
        public static void Main()
        {
            MemoryManager.AttachApp();
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Settings.Default.PropertyChanged += (o, x) => Settings.Default.Save();
            Variables.GenerateTheme();
        }
    }
}

