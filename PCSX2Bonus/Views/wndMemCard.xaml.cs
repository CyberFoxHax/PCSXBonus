using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using PCSX2Bonus.Properties;
using Extensions = PCSX2Bonus.PCSX2Bonus.Extensions;

namespace PCSX2Bonus.Views {
	public sealed partial class wndMemCard {
		internal System.Windows.Controls.Button btnBrowse;
		internal System.Windows.Controls.Button btnCancel;
		internal System.Windows.Controls.Button btnOk;
		private PCSX2Bonus.Game g;
		internal System.Windows.Controls.ListView lvMemCards;
		private PCSX2Bonus.IniFile _pcsx2Ui;
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
			foreach (var newItem in Directory.GetFiles(dialog.SelectedPath, "*.ps2").Select(str => new FileInfo(str)).Select(info => new PCSX2Bonus.MemoryCard {
				Name = info.Name,
				WriteTime = info.LastWriteTime.ToShortDateString(),
				Size = PCSX2Bonus.Tools.GetSizeReadable2(info.Length)
			})) {
				lvMemCards.Items.Add(newItem);
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (!Extensions.IsEmpty(tbCardPath.Text)) {
				if (lvMemCards.SelectedItems.Count == 0) {
					PCSX2Bonus.Tools.ShowMessage("You must select a card to assign", PCSX2Bonus.MessageType.Error);
				}
				else {
					_pcsx2Ui.Write("Folders", "UseDefaultMemoryCards", "disabled");
					var iNIPath = PCSX2Bonus.UserSettings.ConfigDir + @"\" + g.FileSafeTitle + @"\PCSX2_ui.ini";
					var selectedItem = (PCSX2Bonus.MemoryCard)lvMemCards.SelectedItem;
					var file = new PCSX2Bonus.IniFile(iNIPath);
					file.Write("MemoryCards", "Slot1_Enable", "enabled");
					file.Write("MemoryCards", "Slot1_Filename", selectedItem.Name);
					file.Write("Folders", "MemoryCards", Extensions.Escape(tbCardPath.Text));
					PCSX2Bonus.Tools.ShowMessage("Successfully assigned and enabled " + selectedItem.Name + " to slot 1\n for the game " + g.Title, PCSX2Bonus.MessageType.Info);
					Close();
				}
			}
			else {
				PCSX2Bonus.Tools.ShowMessage("The selected memory card cannot be null", PCSX2Bonus.MessageType.Error);
			}
		}

		private void LoadInitialCard() {
			var path = Extensions.Unescape(_pcsx2Ui.Read("Folders", "MemoryCards"));
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
			foreach (var newItem in Directory.GetFiles(tbCardPath.Text, "*.ps2").Select(str2 => new FileInfo(str2)).Select(info => new PCSX2Bonus.MemoryCard {
				Name = info.Name,
				WriteTime = info.LastWriteTime.ToShortDateString(),
				Size = PCSX2Bonus.Tools.GetSizeReadable2(info.Length)
			})) {
				lvMemCards.Items.Add(newItem);
			}
		}

		private void Setup() {
			g = (PCSX2Bonus.Game)Tag;
			Title = "Memory Card Management [" + g.Title + "]";
			_pcsx2Ui = new PCSX2Bonus.IniFile(Path.Combine(PCSX2Bonus.UserSettings.ConfigDir, Path.Combine(g.FileSafeTitle, "PCSX2_ui.ini")));
			LoadInitialCard();
		}

		private void wndMemCard_Loaded(object sender, RoutedEventArgs e) {
			Setup();
		}
	}
}

