using System.Linq;

namespace PCSX2Bonus {
	using Properties;
	using System;
	using System.CodeDom.Compiler;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.IO;
	using System.Windows;
	using System.Windows.Forms;
	using System.Windows.Markup;

	public sealed class wndMemCard : Window, IComponentConnector {
		private bool _contentLoaded;
		internal System.Windows.Controls.Button btnBrowse;
		internal System.Windows.Controls.Button btnCancel;
		internal System.Windows.Controls.Button btnOk;
		private Game g;
		internal System.Windows.Controls.ListView lvMemCards;
		private IniFile _pcsx2Ui;
		internal System.Windows.Controls.TextBox tbCardPath;

		public wndMemCard() {
			InitializeComponent();
			Owner = System.Windows.Application.Current.MainWindow;
			Loaded += wndMemCard_Loaded;
		}

		private void btnBrowse_Click(object sender, RoutedEventArgs e) {
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
			tbCardPath.Text = dialog.SelectedPath;
			lvMemCards.Items.Clear();
			foreach (var newItem in Directory.GetFiles(dialog.SelectedPath, "*.ps2").Select(str => new FileInfo(str)).Select(info => new MemoryCard {
				Name = info.Name,
				WriteTime = info.LastWriteTime.ToShortDateString(),
				Size = Tools.GetSizeReadable2(info.Length)
			})){
				lvMemCards.Items.Add(newItem);
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (!tbCardPath.Text.IsEmpty()) {
				if (lvMemCards.SelectedItems.Count == 0) {
					Tools.ShowMessage("You must select a card to assign", MessageType.Error);
				}
				else {
					_pcsx2Ui.Write("Folders", "UseDefaultMemoryCards", "disabled");
					var iNIPath = UserSettings.ConfigDir + @"\" + g.FileSafeTitle + @"\PCSX2_ui.ini";
					var selectedItem = (MemoryCard)lvMemCards.SelectedItem;
					var file = new IniFile(iNIPath);
					file.Write("MemoryCards", "Slot1_Enable", "enabled");
					file.Write("MemoryCards", "Slot1_Filename", selectedItem.Name);
					file.Write("Folders", "MemoryCards", tbCardPath.Text.Escape());
					Tools.ShowMessage("Successfully assigned and enabled " + selectedItem.Name + " to slot 1\n for the game " + g.Title, MessageType.Info);
					Close();
				}
			}
			else {
				Tools.ShowMessage("The selected memory card cannot be null", MessageType.Error);
			}
		}

		[GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
		public void InitializeComponent() {
			if (_contentLoaded) return;
			_contentLoaded = true;
			var resourceLocator = new Uri("/PCSX2Bonus;component/gui/wndmemcard.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}

		private void LoadInitialCard() {
			var path = _pcsx2Ui.Read("Folders", "MemoryCards").Unescape();
			if (path == "memcards") {
				path = Path.Combine(Settings.Default.pcsx2DataDir, "memcards");
			}
			if (!Directory.Exists(path)) {
				var dialog = new FolderBrowserDialog {
					Description = "Select the directory containing the memory card files"
				};
				if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) {
					Close();
					return;
				}
				tbCardPath.Text = dialog.SelectedPath;
			}
			else {
				tbCardPath.Text = path;
			}
			foreach (var newItem in Directory.GetFiles(tbCardPath.Text, "*.ps2").Select(str2 => new FileInfo(str2)).Select(info => new MemoryCard {
				Name = info.Name,
				WriteTime = info.LastWriteTime.ToShortDateString(),
				Size = Tools.GetSizeReadable2(info.Length)
			})){
				lvMemCards.Items.Add(newItem);
			}
		}

		private void Setup() {
			g = (Game)Tag;
			Title = "Memory Card Management [" + g.Title + "]";
			_pcsx2Ui = new IniFile(Path.Combine(UserSettings.ConfigDir, Path.Combine(g.FileSafeTitle, "PCSX2_ui.ini")));
			LoadInitialCard();
		}

		[DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target) {
			switch (connectionId) {
				case 1:
					tbCardPath = (System.Windows.Controls.TextBox)target;
					return;

				case 2:
					btnBrowse = (System.Windows.Controls.Button)target;
					btnBrowse.Click += btnBrowse_Click;
					return;

				case 3:
					lvMemCards = (System.Windows.Controls.ListView)target;
					return;

				case 4:
					btnCancel = (System.Windows.Controls.Button)target;
					btnCancel.Click += btnCancel_Click;
					return;

				case 5:
					btnOk = (System.Windows.Controls.Button)target;
					btnOk.Click += btnOk_Click;
					return;
			}
			_contentLoaded = true;
		}

		private void wndMemCard_Loaded(object sender, RoutedEventArgs e) {
			Setup();
		}
	}
}

