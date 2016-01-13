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

    public sealed class wndShaderConfig : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal Button btnAddNew;
        internal Button btnCancel;
        internal Button btnOk;
        private Game g;
        internal ListBox lbShaders;
        private IniFile pcsx2_ini;

        public wndShaderConfig()
        {
            this.InitializeComponent();
            base.Owner = Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndShaderConfig_Loaded);
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog {
                Filter = "Shader Files | *.fx"
            };
            if (dialog.ShowDialog() == true)
            {
                ListViewItem newItem = new ListViewItem {
                    Content = dialog.FileName.FileNameNoExt(),
                    Tag = Path.Combine(UserSettings.ShadersDir, Path.GetFileName(dialog.FileName))
                };
                this.lbShaders.Items.Add(newItem);
                try
                {
                    File.Copy(dialog.FileName, Path.Combine(UserSettings.ShadersDir, Path.GetFileName(dialog.FileName)), true);
                }
                catch
                {
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.lbShaders.SelectedItems.Count == 0)
            {
                Tools.ShowMessage("A shader must be selected!", MessageType.Error);
            }
            else
            {
                this.pcsx2_ini.Read("Shader", "Default");
                ListViewItem selectedItem = (ListViewItem) this.lbShaders.SelectedItem;
                this.pcsx2_ini.Write("Shader", "Default", selectedItem.Tag.ToString());
                base.Close();
            }
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndshaderconfig.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        private void Setup()
        {
            this.g = (Game) base.Tag;
            this.lbShaders.DisplayMemberPath = "Text";
            this.pcsx2_ini = new IniFile(Path.Combine(UserSettings.ConfigDir, this.g.FileSafeTitle) + @"\PCSX2Bonus.ini");
            foreach (string str in Directory.GetFiles(UserSettings.ShadersDir))
            {
                ListViewItem newItem = new ListViewItem {
                    Content = str.FileNameNoExt(),
                    Tag = str
                };
                this.lbShaders.Items.Add(newItem);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.lbShaders = (ListBox) target;
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

        private void wndShaderConfig_Loaded(object sender, RoutedEventArgs e)
        {
            this.Setup();
        }
    }
}

