using System.Windows;
using System.Windows.Controls;

namespace PCSX2Bonus.Views {
	public sealed partial class wndAbout{
		internal Button btnOk;
		internal StackPanel spThanks;

		public wndAbout() {
			this.InitializeComponent();
			base.Owner = Application.Current.MainWindow;
			base.Loaded += this.wndAbout_Loaded;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			base.Close();
		}

		private void wndAbout_Loaded(object sender, RoutedEventArgs e) {
		}
	}
}
