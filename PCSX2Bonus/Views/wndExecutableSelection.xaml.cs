using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Extensions = PCSX2Bonus.PCSX2Bonus.Extensions;

namespace PCSX2Bonus.Views {
	public sealed partial class wndExecutableSelection{
		internal Button btnAddNew;
		internal Button btnCancel;
		internal Button btnOk;
		private PCSX2Bonus.Game g;
		internal ListBox lbVersions;
		private string original = string.Empty;
		private PCSX2Bonus.IniFile pcsx2_ini;
		private string selectedExe = string.Empty;

		public wndExecutableSelection() {
			InitializeComponent();
			Owner = Application.Current.MainWindow;
			Loaded += wndExecutableSelection_Loaded;
		}

		private void btnAddNew_Click(object sender, RoutedEventArgs e) {
			var dialog = new OpenFileDialog {
				Filter = "Executables | *.exe"
			};
			if (dialog.ShowDialog() != true) return;
			lbVersions.Items.Clear();
			lbVersions.Items.Add(Path.GetFileNameWithoutExtension(dialog.FileName));
			selectedExe = dialog.FileName;
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (lbVersions.SelectedItems.Count == 0) {
				PCSX2Bonus.Tools.ShowMessage("A PCSX2 version must be selected", PCSX2Bonus.MessageType.Error);
			}
			else{
				pcsx2_ini.Write("Additional Executables", "Default",
					Extensions.IsEmpty(selectedExe) ? original : selectedExe);
				Close();
			}
		}

		private void Setup() {
			g = (PCSX2Bonus.Game)base.Tag;
			pcsx2_ini = new PCSX2Bonus.IniFile(Path.Combine(PCSX2Bonus.UserSettings.ConfigDir, g.FileSafeTitle) + @"\PCSX2Bonus.ini");
			lbVersions.Items.Add(Path.GetFileNameWithoutExtension(pcsx2_ini.Read("Additional Executables", "Default")));
			original = pcsx2_ini.Read("Additional Executables", "Default");
		}

		private void wndExecutableSelection_Loaded(object sender, RoutedEventArgs e) {
			Setup();
		}
	}
}
