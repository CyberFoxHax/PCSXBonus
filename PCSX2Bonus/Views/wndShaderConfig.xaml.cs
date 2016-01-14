﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Linq;
using Extensions = PCSX2Bonus.PCSX2Bonus.Extensions;

namespace PCSX2Bonus.Views {
	public sealed partial class wndShaderConfig {
		internal Button btnAddNew;
		internal Button btnCancel;
		internal Button btnOk;
		private PCSX2Bonus.Game g;
		internal ListBox lbShaders;
		private PCSX2Bonus.IniFile pcsx2_ini;

		public wndShaderConfig() {
			InitializeComponent();
			Owner = Application.Current.MainWindow;
			Loaded += wndShaderConfig_Loaded;
		}

		private void btnAddNew_Click(object sender, RoutedEventArgs e) {
			var dialog = new OpenFileDialog {
				Filter = "Shader Files | *.fx"
			};
			if (dialog.ShowDialog() != true) return;
			var newItem = new ListViewItem {
				Content = Extensions.FileNameNoExt(dialog.FileName),
				Tag = Path.Combine(PCSX2Bonus.UserSettings.ShadersDir, Path.GetFileName(dialog.FileName))
			};
			lbShaders.Items.Add(newItem);
			try {
				File.Copy(dialog.FileName, Path.Combine(PCSX2Bonus.UserSettings.ShadersDir, Path.GetFileName(dialog.FileName)), true);
			}
			catch {
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			if (lbShaders.SelectedItems.Count == 0) {
				PCSX2Bonus.Tools.ShowMessage("A shader must be selected!", PCSX2Bonus.MessageType.Error);
			}
			else {
				pcsx2_ini.Read("Shader", "Default");
				var selectedItem = (ListViewItem)lbShaders.SelectedItem;
				pcsx2_ini.Write("Shader", "Default", selectedItem.Tag.ToString());
				Close();
			}
		}

		private void Setup() {
			g = (PCSX2Bonus.Game)Tag;
			lbShaders.DisplayMemberPath = "Text";
			pcsx2_ini = new PCSX2Bonus.IniFile(Path.Combine(PCSX2Bonus.UserSettings.ConfigDir, g.FileSafeTitle) + @"\PCSX2Bonus.ini");
			foreach (var newItem in Directory.GetFiles(PCSX2Bonus.UserSettings.ShadersDir).Select(str => new ListViewItem {
				Content = Extensions.FileNameNoExt(str),
				Tag = str
			})){
				lbShaders.Items.Add(newItem);
			}
		}

		private void wndShaderConfig_Loaded(object sender, RoutedEventArgs e) {
			Setup();
		}
	}
}

