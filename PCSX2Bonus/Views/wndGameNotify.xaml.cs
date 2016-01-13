using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using Extensions = PCSX2Bonus.PCSX2Bonus.Extensions;

namespace PCSX2Bonus.Views {
	public sealed partial class wndGameNotify{
		public wndGameNotify() {
			InitializeComponent();
			Topmost = true;
			Opacity = 0.0;
			Loaded += wndGameNotify_Loaded;
		}

		private async void SlideIn() {
			Opacity = 1.0;
			var workingArea = Screen.PrimaryScreen.WorkingArea;
			var aTop = new DoubleAnimation {
				From = workingArea.Bottom + Height,
				To = (workingArea.Bottom - Height) - 5.0,
				Duration = new Duration(TimeSpan.FromSeconds(0.5))
			};
			var aBottom = new DoubleAnimation {
				To = (workingArea.Bottom + Height) + 5.0,
				Duration = new Duration(TimeSpan.FromSeconds(0.5))
			};
			var storyboard = new Storyboard();
			storyboard.Children.Add(aTop);
			Storyboard.SetTarget(aTop, this);
			Storyboard.SetTargetProperty(aTop, new PropertyPath(TopProperty));
			await Extensions.BeginAsync(storyboard);
			await Task.Delay(0xbb8);
			storyboard.Children.Clear();
			storyboard.Children.Add(aBottom);
			Storyboard.SetTarget(aBottom, this);
			Storyboard.SetTargetProperty(aBottom, new PropertyPath(TopProperty));
			await Extensions.BeginAsync(storyboard);
			Close();
		}

		private void wndGameNotify_Loaded(object sender, RoutedEventArgs e) {
			var workingArea = Screen.FromHandle(new WindowInteropHelper(this).Handle).WorkingArea;
			base.Left = (workingArea.Right - base.Width) - 5.0;
			SlideIn();
		}

		private void wndGameNotify_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
			base.Close();
		}

	}
}

