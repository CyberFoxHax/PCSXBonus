namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Markup;

    public sealed class wndMemCard : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal System.Windows.Controls.Button btnBrowse;
        internal System.Windows.Controls.Button btnCancel;
        internal System.Windows.Controls.Button btnOk;
        private Game g;
        internal System.Windows.Controls.ListView lvMemCards;
        private IniFile pcsx2_ui;
        internal System.Windows.Controls.TextBox tbCardPath;

        public wndMemCard()
        {
            this.InitializeComponent();
            base.Owner = System.Windows.Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndMemCard_Loaded);
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.tbCardPath.Text = dialog.SelectedPath;
                this.lvMemCards.Items.Clear();
                foreach (string str in Directory.GetFiles(dialog.SelectedPath, "*.ps2"))
                {
                    FileInfo info = new FileInfo(str);
                    MemoryCard newItem = new MemoryCard {
                        Name = info.Name,
                        WriteTime = info.LastWriteTime.ToShortDateString(),
                        Size = Tools.GetSizeReadable2(info.Length)
                    };
                    this.lvMemCards.Items.Add(newItem);
                }
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            base.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!this.tbCardPath.Text.IsEmpty())
            {
                if (this.lvMemCards.SelectedItems.Count == 0)
                {
                    Tools.ShowMessage("You must select a card to assign", MessageType.Error);
                }
                else
                {
                    this.pcsx2_ui.Write("Folders", "UseDefaultMemoryCards", "disabled");
                    string iNIPath = UserSettings.ConfigDir + @"\" + this.g.FileSafeTitle + @"\PCSX2_ui.ini";
                    MemoryCard selectedItem = (MemoryCard) this.lvMemCards.SelectedItem;
                    IniFile file = new IniFile(iNIPath);
                    file.Write("MemoryCards", "Slot1_Enable", "enabled");
                    file.Write("MemoryCards", "Slot1_Filename", selectedItem.Name);
                    file.Write("Folders", "MemoryCards", this.tbCardPath.Text.Escape());
                    Tools.ShowMessage("Successfully assigned and enabled " + selectedItem.Name + " to slot 1\n for the game " + this.g.Title, MessageType.Info);
                    base.Close();
                }
            }
            else
            {
                Tools.ShowMessage("The selected memory card cannot be null", MessageType.Error);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndmemcard.xaml", UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocator);
            }
        }

        private void LoadInitialCard()
        {
            string path = this.pcsx2_ui.Read("Folders", "MemoryCards").Unescape();
            if (path == "memcards")
            {
                path = Path.Combine(Settings.Default.pcsx2DataDir, "memcards");
            }
            if (!Directory.Exists(path))
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog {
                    Description = "Select the directory containing the memory card files"
                };
                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    base.Close();
                    return;
                }
                this.tbCardPath.Text = dialog.SelectedPath;
            }
            else
            {
                this.tbCardPath.Text = path;
            }
            foreach (string str2 in Directory.GetFiles(this.tbCardPath.Text, "*.ps2"))
            {
                FileInfo info = new FileInfo(str2);
                MemoryCard newItem = new MemoryCard {
                    Name = info.Name,
                    WriteTime = info.LastWriteTime.ToShortDateString(),
                    Size = Tools.GetSizeReadable2(info.Length)
                };
                this.lvMemCards.Items.Add(newItem);
            }
        }

        private void Setup()
        {
            this.g = (Game) base.Tag;
            base.Title = "Memory Card Management [" + this.g.Title + "]";
            this.pcsx2_ui = new IniFile(Path.Combine(UserSettings.ConfigDir, Path.Combine(this.g.FileSafeTitle, "PCSX2_ui.ini")));
            this.LoadInitialCard();
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.tbCardPath = (System.Windows.Controls.TextBox) target;
                    return;

                case 2:
                    this.btnBrowse = (System.Windows.Controls.Button) target;
                    this.btnBrowse.Click += new RoutedEventHandler(this.btnBrowse_Click);
                    return;

                case 3:
                    this.lvMemCards = (System.Windows.Controls.ListView) target;
                    return;

                case 4:
                    this.btnCancel = (System.Windows.Controls.Button) target;
                    this.btnCancel.Click += new RoutedEventHandler(this.btnCancel_Click);
                    return;

                case 5:
                    this.btnOk = (System.Windows.Controls.Button) target;
                    this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndMemCard_Loaded(object sender, RoutedEventArgs e)
        {
            this.Setup();
        }
    }
}

