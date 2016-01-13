namespace PCSX2Bonus {
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

	public sealed class wndSettings : Window, IComponentConnector {
		private bool _contentLoaded;
		private Gamepad _gamepad;
		internal System.Windows.Controls.Button btnBrowseData;
		internal System.Windows.Controls.Button btnBrowseDir;
		internal System.Windows.Controls.Button btnBrowseExe;
		internal System.Windows.Controls.Button btnCancel;
		internal System.Windows.Controls.Button btnCancelSet;
		internal System.Windows.Controls.Button btnConfirmSet;
		internal System.Windows.Controls.Button btnEditTheme;
		internal System.Windows.Controls.Button btnOk;
		internal System.Windows.Controls.CheckBox cbEnableGamepad;
		internal System.Windows.Controls.CheckBox cbSaveInfo;
		internal System.Windows.Controls.ComboBox cbSortType;
		internal System.Windows.Controls.ComboBox cbTheme;
		internal System.Windows.Controls.CheckBox cbUseGameToast;
		internal System.Windows.Controls.CheckBox cbUseUpdated;
		internal System.Windows.Controls.ComboBox cbViewType;
		internal System.Windows.Controls.TextBox tbButtonCancel;
		internal System.Windows.Controls.TextBox tbButtonComfirm;
		internal System.Windows.Controls.TextBox tbPcsx2DataDir;
		internal System.Windows.Controls.TextBox tbPcsx2Dir;
		internal System.Windows.Controls.TextBox tbPcsx2Exe;

		public wndSettings() {
			this.InitializeComponent();
			base.Owner = System.Windows.Application.Current.MainWindow;
			base.Loaded += new RoutedEventHandler(this.wndSettings_Loaded);
		}

		private void btnBrowseData_Click(object sender, RoutedEventArgs e) {
			FolderBrowserDialog dialog = new FolderBrowserDialog {
				Description = "Select the directory containing the PCSX2 data folders (bios, inis, logs, memcards, snaps, sstates)"
			};
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				string[] first = new string[] { "inis", "bios", "logs", "memcards", "snaps", "sstates" };
				string[] second = (from d in Directory.GetDirectories(dialog.SelectedPath) select new DirectoryInfo(d).Name).ToArray<string>();
				if (first.Except<string>(second).Any<string>()) {
					Tools.ShowMessage("A required folder has not been found!", MessageType.Error);
				}
				else {
					this.tbPcsx2DataDir.Text = dialog.SelectedPath;
				}
			}
		}

		private void btnBrowseDir_Click(object sender, RoutedEventArgs e) {
			FolderBrowserDialog dialog = new FolderBrowserDialog {
				Description = "Select the directory containing PCSX2"
			};
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				this.tbPcsx2Dir.Text = dialog.SelectedPath;
			}
		}

		private void btnBrowseExe_Click(object sender, RoutedEventArgs e) {
			Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog {
				Filter = "Executables | *.exe",
				Multiselect = false
			};
			if (dialog.ShowDialog() == true) {
				this.tbPcsx2Exe.Text = dialog.FileName;
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			base.Close();
		}

		private void btnCancelSet_Click(object sender, RoutedEventArgs e) {
			base.IsHitTestVisible = false;
			this.tbButtonCancel.Text = "Waiting...";
			this._gamepad.PollAsync();
			EventHandler _handler = null;
			_handler = (o, x) => this.Dispatcher.Invoke(delegate {
				Settings.Default.buttonCancel = (int)o;
				this._gamepad.CancelPollAsync();
				this._gamepad.ButtonPressed -= _handler;
				this.IsHitTestVisible = true;
			});
			this._gamepad.ButtonPressed += _handler;
		}

		private void btnConfirmSet_Click(object sender, RoutedEventArgs e) {
			base.IsHitTestVisible = false;
			this.tbButtonComfirm.Text = "Waiting...";
			this._gamepad.PollAsync();
			EventHandler _handler = null;
			_handler = (o, x) => this.Dispatcher.Invoke(delegate {
				Settings.Default.buttonOk = (int)o;
				this._gamepad.CancelPollAsync();
				this._gamepad.ButtonPressed -= _handler;
				this.IsHitTestVisible = true;
			});
			this._gamepad.ButtonPressed += _handler;
		}

		private void btnEditTheme_Click(object sender, RoutedEventArgs e) {
			if (this.cbTheme.SelectedItem == null) {
				Tools.ShowMessage("No theme selected", MessageType.Error);
			}
			else {
				string xmlFile = this.cbTheme.SelectedItem.ToString();
				UserStyles.LoadTheme(xmlFile);
				new wndThemeEditor { Tag = Path.Combine(UserSettings.ThemesDir, xmlFile) }.Show();
			}
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			Settings.Default.defaultSort = (((ComboBoxItem)this.cbSortType.SelectedItem).Content.ToString() != "Unsorted") ? ((ComboBoxItem)this.cbSortType.SelectedItem).Content.ToString() : "Default";
			Settings.Default.defaultView = ((ComboBoxItem)this.cbViewType.SelectedItem).Content.ToString();
			Settings.Default.saveInfo = this.cbSaveInfo.IsChecked.Value;
			Settings.Default.useUpdatedCompat = this.cbUseUpdated.IsChecked.Value;
			Settings.Default.pcsx2Dir = this.tbPcsx2Dir.Text;
			Settings.Default.pcsx2DataDir = this.tbPcsx2DataDir.Text;
			Settings.Default.pcsx2Exe = this.tbPcsx2Exe.Text;
			Settings.Default.enableGamepad = this.cbEnableGamepad.IsChecked.Value;
			Settings.Default.defaultTheme = this.cbTheme.SelectedItem.ToString();
			Settings.Default.enableGameToast = this.cbUseGameToast.IsChecked.Value;
			base.Close();
		}

		private void cbEnableGamepad_Checked(object sender, RoutedEventArgs e) {
			if (this.cbEnableGamepad.IsChecked.Value) {
				this._gamepad = new Gamepad(this);
				if (!this._gamepad.IsValid) {
					this.cbEnableGamepad.IsChecked = false;
					Tools.ShowMessage("Error enabling gamepad", MessageType.Error);
				}
			}
			else {
				Tools.ShowMessage("Error enabling gamepad [Error code: 10045]", MessageType.Error);
				this.cbEnableGamepad.IsChecked = false;
			}
		}

		private void FetchThemes() {
			foreach (string str in Directory.GetFiles(UserSettings.ThemesDir, "*.xml").ToArray<string>()) {
				this.cbTheme.Items.Add(Path.GetFileName(str));
			}
			this.cbTheme.SelectedItem = Settings.Default.defaultTheme;
		}

		[DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent() {
			if (!this._contentLoaded) {
				this._contentLoaded = true;
				Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndsettings.xaml", UriKind.Relative);
				System.Windows.Application.LoadComponent(this, resourceLocator);
			}
		}

		private void Setup() {
			this.FetchThemes();
			string defaultView = Settings.Default.defaultView;
			if (defaultView != null) {
				if (!(defaultView == "Stacked")) {
					if (defaultView == "Tile") {
						this.cbViewType.SelectedIndex = 1;
					}
					else if (defaultView == "TV") {
						this.cbViewType.SelectedIndex = 2;
					}
				}
				else {
					this.cbViewType.SelectedIndex = 0;
				}
			}
			string defaultSort = Settings.Default.defaultSort;
			if (defaultSort != null) {
				if (!(defaultSort == "Alphabetical")) {
					if (!(defaultSort == "Serial")) {
						if (defaultSort == "Default") {
							this.cbSortType.SelectedIndex = 2;
						}
						return;
					}
				}
				else {
					this.cbSortType.SelectedIndex = 0;
					return;
				}
				this.cbSortType.SelectedIndex = 1;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 1:
					((wndSettings)target).Closing += new CancelEventHandler(this.Window_Closing);
					return;

				case 2:
					this.cbViewType = (System.Windows.Controls.ComboBox)target;
					return;

				case 3:
					this.cbSortType = (System.Windows.Controls.ComboBox)target;
					return;

				case 4:
					this.cbUseUpdated = (System.Windows.Controls.CheckBox)target;
					return;

				case 5:
					this.cbSaveInfo = (System.Windows.Controls.CheckBox)target;
					return;

				case 6:
					this.cbUseGameToast = (System.Windows.Controls.CheckBox)target;
					return;

				case 7:
					this.tbPcsx2Dir = (System.Windows.Controls.TextBox)target;
					return;

				case 8:
					this.btnBrowseDir = (System.Windows.Controls.Button)target;
					this.btnBrowseDir.Click += new RoutedEventHandler(this.btnBrowseDir_Click);
					return;

				case 9:
					this.tbPcsx2DataDir = (System.Windows.Controls.TextBox)target;
					return;

				case 10:
					this.btnBrowseData = (System.Windows.Controls.Button)target;
					this.btnBrowseData.Click += new RoutedEventHandler(this.btnBrowseData_Click);
					return;

				case 11:
					this.tbPcsx2Exe = (System.Windows.Controls.TextBox)target;
					return;

				case 12:
					this.btnBrowseExe = (System.Windows.Controls.Button)target;
					this.btnBrowseExe.Click += new RoutedEventHandler(this.btnBrowseExe_Click);
					return;

				case 13:
					this.cbTheme = (System.Windows.Controls.ComboBox)target;
					return;

				case 14:
					this.btnEditTheme = (System.Windows.Controls.Button)target;
					this.btnEditTheme.Click += new RoutedEventHandler(this.btnEditTheme_Click);
					return;

				case 15:
					this.cbEnableGamepad = (System.Windows.Controls.CheckBox)target;
					this.cbEnableGamepad.Checked += new RoutedEventHandler(this.cbEnableGamepad_Checked);
					return;

				case 0x10:
					this.tbButtonComfirm = (System.Windows.Controls.TextBox)target;
					return;

				case 0x11:
					this.btnConfirmSet = (System.Windows.Controls.Button)target;
					this.btnConfirmSet.Click += new RoutedEventHandler(this.btnConfirmSet_Click);
					return;

				case 0x12:
					this.tbButtonCancel = (System.Windows.Controls.TextBox)target;
					return;

				case 0x13:
					this.btnCancelSet = (System.Windows.Controls.Button)target;
					this.btnCancelSet.Click += new RoutedEventHandler(this.btnCancelSet_Click);
					return;

				case 20:
					this.btnCancel = (System.Windows.Controls.Button)target;
					this.btnCancel.Click += new RoutedEventHandler(this.btnCancel_Click);
					return;

				case 0x15:
					this.btnOk = (System.Windows.Controls.Button)target;
					this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
					return;
			}
			this._contentLoaded = true;
		}

		private void Window_Closing(object sender, CancelEventArgs e) {
			base.DialogResult = true;
			if (this._gamepad != null) {
				this._gamepad.CancelPollAsync();
				this._gamepad.Dispose();
			}
		}

		private void wndSettings_Loaded(object sender, RoutedEventArgs e) {
			this.Setup();
		}
	}
}

