using System.Windows;

namespace PCSX2Bonus.Views {
	public sealed partial class wndAbout{
		public wndAbout() {
			InitializeComponent();
			Owner = Application.Current.MainWindow;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			Close();
		}
	}
}
