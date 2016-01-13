namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Markup;

    public sealed class wndSetup : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal System.Windows.Controls.Button btnBrowse;
        internal System.Windows.Controls.Button btnBrowseData;
        internal System.Windows.Controls.Button btnOk;
        private bool setupCompleted;
        internal System.Windows.Controls.TextBox tbPcsx2DataDir;
        internal System.Windows.Controls.TextBox tbPcsx2Dir;

        public wndSetup()
        {
            this.InitializeComponent();
            base.Loaded += new RoutedEventHandler(this.wndSetup_Loaded);
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                Description = "Select the directory containing PCSX2"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                bool flag = false;
                string str = string.Empty;
                foreach (string str2 in Directory.GetFiles(dialog.SelectedPath, "*.exe"))
                {
                    if ((str2.Contains("pcsx2-r", StringComparison.InvariantCultureIgnoreCase) && !str2.Contains("Uninst", StringComparison.InvariantCultureIgnoreCase)) || Path.GetFileName(str2).Equals("pcsx2.exe", StringComparison.InvariantCultureIgnoreCase))
                    {
                        flag = true;
                        str = str2;
                        break;
                    }
                }
                if (!flag)
                {
                    Tools.ShowMessage("PCSX2 executable could not be found!", MessageType.Error);
                }
                else
                {
                    Settings.Default.pcsx2Exe = str;
                    this.tbPcsx2Dir.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnBrowseData_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog {
                Description = "Select the directory containing the PCSX2 data folders (bios, inis, logs, memcards, snaps, sstates)"
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string[] first = new string[] { "inis", "bios", "logs", "memcards", "snaps", "sstates" };
                string[] second = (from d in Directory.GetDirectories(dialog.SelectedPath) select new DirectoryInfo(d).Name).ToArray<string>();
                if (first.Except<string>(second).Any<string>())
                {
                    Tools.ShowMessage("A required folder has not been found!", MessageType.Error);
                }
                else
                {
                    this.tbPcsx2DataDir.Text = dialog.SelectedPath;
                }
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbPcsx2DataDir.Text.IsEmpty() || this.tbPcsx2Dir.Text.IsEmpty())
            {
                Tools.ShowMessage("Required fields cannot be empty!", MessageType.Error);
            }
            else
            {
                Settings.Default.pcsx2Dir = this.tbPcsx2Dir.Text;
                Settings.Default.pcsx2DataDir = this.tbPcsx2DataDir.Text;
                this.setupCompleted = true;
                base.Close();
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndsetup.xaml", UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocator);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.tbPcsx2Dir = (System.Windows.Controls.TextBox) target;
                    return;

                case 2:
                    this.btnBrowse = (System.Windows.Controls.Button) target;
                    this.btnBrowse.Click += new RoutedEventHandler(this.btnBrowse_Click);
                    return;

                case 3:
                    this.tbPcsx2DataDir = (System.Windows.Controls.TextBox) target;
                    return;

                case 4:
                    this.btnBrowseData = (System.Windows.Controls.Button) target;
                    this.btnBrowseData.Click += new RoutedEventHandler(this.btnBrowseData_Click);
                    return;

                case 5:
                    this.btnOk = (System.Windows.Controls.Button) target;
                    this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndSetup_Closing(object sender, CancelEventArgs e)
        {
            if (!this.setupCompleted)
            {
                if (System.Windows.MessageBox.Show("Setup has not been completed, exit?", "PCSX2Bonus", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    System.Windows.Application.Current.Shutdown();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void wndSetup_Loaded(object sender, RoutedEventArgs e)
        {
            base.Closing += new CancelEventHandler(this.wndSetup_Closing);
        }
    }
}

