using System;

namespace PCSX2Bonus{
	public partial class App{
		public App(){
			var window = new Views.MainWindow();
			window.Show();

#if !DEBUG
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				var newWindow = new Views.wndErrorReport((Exception)e.ExceptionObject, window);
				newWindow.ShowDialog();
			};
#endif
		}
	}
}