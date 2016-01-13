namespace PCSX2Bonus
{
    using Microsoft.Win32;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public sealed class wndExecutableSelection : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal Button btnAddNew;
        internal Button btnCancel;
        internal Button btnOk;
        private Game g;
        internal ListBox lbVersions;
        private string original = string.Empty;
        private IniFile pcsx2_ini;
        private string selectedExe = string.Empty;

        public wndExecutableSelection()
        {
            this.InitializeComponent();
            base.Owner = Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndExecutableSelection_Loaded);
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "Executables | *.exe"
            };
            if (dialog.ShowDialog() == true)
            {
                this.lbVersions.Items.Clear();
                this.lbVersions.Items.Add(Path.GetFileNameWithoutExtension(dialog.FileName));
                this.selectedExe = dialog.FileName;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbVersions.SelectedItems.Count == 0)
            {
                Tools.ShowMessage("A PCSX2 version must be selected", MessageType.Error);
            }
            else
            {
                if (this.selectedExe.IsEmpty())
                {
                    this.pcsx2_ini.Write("Additional Executables", "Default", this.original);
                }
                else
                {
                    this.pcsx2_ini.Write("Additional Executables", "Default", this.selectedExe);
                }
                base.Close();
            }
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndexecutableselection.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void Setup()
        {
            this.g = (Game) base.Tag;
            this.pcsx2_ini = new IniFile(Path.Combine(UserSettings.ConfigDir, this.g.FileSafeTitle) + @"\PCSX2Bonus.ini");
            this.lbVersions.Items.Add(Path.GetFileNameWithoutExtension(this.pcsx2_ini.Read("Additional Executables", "Default")));
            this.original = this.pcsx2_ini.Read("Additional Executables", "Default");
        }

        [EditorBrowsable(EditorBrowsableState.Never), GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.lbVersions = (ListBox) target;
                    return;

                case 2:
                    this.btnCancel = (Button) target;
                    this.btnCancel.Click += new RoutedEventHandler(this.btnCancel_Click);
                    return;

                case 3:
                    this.btnOk = (Button) target;
                    this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
                    return;

                case 4:
                    this.btnAddNew = (Button) target;
                    this.btnAddNew.Click += new RoutedEventHandler(this.btnAddNew_Click);
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndExecutableSelection_Loaded(object sender, RoutedEventArgs e)
        {
            this.Setup();
        }
    }
}

