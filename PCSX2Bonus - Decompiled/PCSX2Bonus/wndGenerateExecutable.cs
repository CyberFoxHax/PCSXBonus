namespace PCSX2Bonus
{
    using Microsoft.CSharp;
    using Microsoft.Win32;
    using PCSX2Bonus.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms;
    using System.Windows.Markup;

    public sealed class wndGenerateExecutable : Window, IComponentConnector
    {
        private bool _contentLoaded;
        internal System.Windows.Controls.Button btnBrowseIcon;
        internal System.Windows.Controls.Button btnBrowseOutPath;
        internal System.Windows.Controls.Button btnCancel;
        internal System.Windows.Controls.Button btnOk;
        internal System.Windows.Controls.CheckBox cbFullBoot;
        internal System.Windows.Controls.CheckBox cbNoDisc;
        internal System.Windows.Controls.CheckBox cbNoGui;
        internal System.Windows.Controls.CheckBox cbNoHacks;
        internal System.Windows.Controls.CheckBox cbUseDefault;
        internal System.Windows.Controls.CheckBox cbUseDefaultIcon;
        private Game g;
        internal System.Windows.Controls.TextBox tbIconPath;
        internal System.Windows.Controls.TextBox tbOutputPath;

        public wndGenerateExecutable()
        {
            this.InitializeComponent();
            base.Owner = System.Windows.Application.Current.MainWindow;
            base.Loaded += new RoutedEventHandler(this.wndGenerateExecutable_Loaded);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (this.tbOutputPath.Text.IsEmpty())
            {
                Tools.ShowMessage("Output path cannot be empty!", MessageType.Error);
            }
            else
            {
                string path = string.Empty;
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    string location = this.g.Location;
                    string text = this.tbOutputPath.Text;
                    string newValue = this.cbNoHacks.IsChecked.Value ? "--nohacks" : string.Empty;
                    string str4 = this.cbNoGui.IsChecked.Value ? "--nogui" : string.Empty;
                    string str5 = this.cbNoDisc.IsChecked.Value ? "--nodisc" : string.Empty;
                    string str6 = this.cbFullBoot.IsChecked.Value ? "--fullboot" : string.Empty;
                    string str7 = this.cbUseDefault.IsChecked.Value ? ("--cfgpath=\"\"" + UserSettings.ConfigDir + @"\" + this.g.FileSafeTitle + "\"\"") : string.Empty;
                    string outputName = this.tbOutputPath.Text + @"\" + this.g.FileSafeTitle + ".exe";
                    Dictionary<string, string> providerOptions = new Dictionary<string, string>();
                    providerOptions.Add("CompilerVersion", "v4.0");
                    CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
                    CompilerParameters options = new CompilerParameters(new string[] { "mscorlib.dll", "System.Core.dll" }, outputName, true) {
                        GenerateExecutable = true
                    };
                    if (this.cbUseDefaultIcon.IsChecked.Value)
                    {
                        path = UserSettings.ImageDir + @"\" + this.g.FileSafeTitle + ".ico";
                        this.CreateIcon(path, true);
                        options.CompilerOptions = string.Format("/target:winexe /optimize /win32icon:{1}{0}{1}", path, "\"");
                    }
                    else if (!this.cbUseDefault.IsChecked.Value)
                    {
                        path = this.tbIconPath.Text;
                        options.CompilerOptions = string.Format("/target:winexe /optimize /win32icon:{1}{0}{1}", path, "\"");
                    }
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        options.CompilerOptions = string.Format("/target:winexe /optimize", new object[0]);
                    }
                    options.ReferencedAssemblies.Add("System.dll");
                    options.IncludeDebugInformation = false;
                    string str9 = Settings.Default.pcsx2Exe;
                    IniFile file = new IniFile(UserSettings.ConfigDir + @"\" + this.g.Title + @"\Settings.ini");
                    if (!string.IsNullOrWhiteSpace(file.Read("Additional Executables", "Default")))
                    {
                        str9 = file.Read("Additional Executables", "Default");
                    }
                    Process process = new Process {
                        StartInfo = { WorkingDirectory = "" }
                    };
                    string directoryName = Path.GetDirectoryName(str9);
                    string str11 = Resources.executableTemplate.Replace("{1}", str9).Replace("{2}", newValue).Replace("{3}", str4).Replace("{4}", str5).Replace("{5}", str6).Replace("{6}", location).Replace("{7}", "\"\"").Replace("{8}", str7).Replace("{9}", directoryName);
                    provider.CompileAssemblyFromSource(options, new string[] { str11 }).Errors.Cast<CompilerError>().ToList<CompilerError>().ForEach(error => Console.WriteLine(error.ErrorText));
                    stopwatch.Stop();
                    System.Windows.MessageBox.Show(string.Concat(new object[] { "Successfully compiled the executable at ", outputName, "\n[", stopwatch.ElapsedMilliseconds, "ms]" }), "PCSX2Bonus", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                catch (Exception exception)
                {
                    Tools.ShowMessage("There was an error building the executable.\nReason: " + exception.Message, MessageType.Error);
                }
                if (File.Exists(path) && Path.GetDirectoryName(path).Contains("PCSX2Bonus"))
                {
                    File.Delete(path);
                }
            }
        }

        private void CreateIcon(string path, bool useDefault = true)
        {
            string str = UserSettings.ImageDir + @"\" + this.g.FileSafeTitle + ".ico";
            if (useDefault)
            {
                using (FileStream stream = new FileStream(str, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    Resources.icon.Save(stream);
                    return;
                }
            }
            Icon icon = new Icon(path);
            using (FileStream stream2 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                icon.Save(stream2);
                icon.Dispose();
            }
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndgenerateexecutable.xaml", UriKind.Relative);
                System.Windows.Application.LoadComponent(this, resourceLocator);
            }
        }

        private void Setup()
        {
            if (Directory.Exists(UserSettings.ConfigDir + @"\" + this.g.Title))
            {
                this.cbUseDefault.IsEnabled = true;
            }
            else
            {
                this.cbUseDefault.IsEnabled = false;
            }
            this.cbUseDefaultIcon.Checked += delegate (object o, RoutedEventArgs e) {
                this.tbIconPath.IsEnabled = false;
                this.btnBrowseIcon.IsEnabled = false;
            };
            this.cbUseDefaultIcon.Unchecked += delegate (object o, RoutedEventArgs e) {
                this.tbIconPath.IsEnabled = true;
                this.btnBrowseIcon.IsEnabled = true;
            };
            this.btnBrowseIcon.Click += delegate (object o, RoutedEventArgs e) {
                Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog {
                    Filter = "Icon Files | *.ico"
                };
                if (dialog.ShowDialog() == true)
                {
                    this.tbIconPath.Text = dialog.FileName;
                }
            };
            this.btnBrowseOutPath.Click += delegate (object o, RoutedEventArgs e) {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    this.tbOutputPath.Text = dialog.SelectedPath;
                }
            };
            this.btnCancel.Click += (o, e) => base.Close();
            this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.cbUseDefault = (System.Windows.Controls.CheckBox) target;
                    return;

                case 2:
                    this.tbIconPath = (System.Windows.Controls.TextBox) target;
                    return;

                case 3:
                    this.btnBrowseIcon = (System.Windows.Controls.Button) target;
                    return;

                case 4:
                    this.cbUseDefaultIcon = (System.Windows.Controls.CheckBox) target;
                    return;

                case 5:
                    this.tbOutputPath = (System.Windows.Controls.TextBox) target;
                    return;

                case 6:
                    this.btnBrowseOutPath = (System.Windows.Controls.Button) target;
                    return;

                case 7:
                    this.cbNoHacks = (System.Windows.Controls.CheckBox) target;
                    return;

                case 8:
                    this.cbNoGui = (System.Windows.Controls.CheckBox) target;
                    return;

                case 9:
                    this.cbNoDisc = (System.Windows.Controls.CheckBox) target;
                    return;

                case 10:
                    this.cbFullBoot = (System.Windows.Controls.CheckBox) target;
                    return;

                case 11:
                    this.btnCancel = (System.Windows.Controls.Button) target;
                    return;

                case 12:
                    this.btnOk = (System.Windows.Controls.Button) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndGenerateExecutable_Loaded(object sender, RoutedEventArgs e)
        {
            this.g = (Game) base.Tag;
            base.Title = string.Format("Generate Executable [{0}]", this.g.Title);
            this.Setup();
        }
    }
}

