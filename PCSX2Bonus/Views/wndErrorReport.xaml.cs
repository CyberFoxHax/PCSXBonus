using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PCSX2Bonus.Views {
	public sealed partial class wndErrorReport {
		private bool _sendingReport;
		private const string body = "";
		internal Button btnOk;
		internal Button btnSend;
		private const string fromPassword = "h@te3027";
		internal TextBlock lblInfo;
		private const string subject = "Error Report";

		public wndErrorReport() {
			InitializeComponent();
			base.Closing += wndErrorReport_Closing;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			Environment.Exit(0);
		}

		private async void btnSend_Click(object sender, RoutedEventArgs e) {
			try {
				_sendingReport = true;
				IsHitTestVisible = false;
				lblInfo.Text = "Sending Report...";
				var from = new MailAddress("miketanner3128@gmail.com", "PCSX2 Tester");
				var to = new MailAddress("miketanner3128@gmail.com", "PCSX2 Developer");
				var smtp = new SmtpClient {
					Host = "smtp.gmail.com",
					Port = 0x24b,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					Credentials = new NetworkCredential(from.Address, "h@te3027"),
					Timeout = 0x4e20
				};
				var asyncVariable0 = new MailMessage(from, to) {
					Subject = "Error Report",
					Body = Message + "\n\n\n" + StackTrace
				};
				using (var message = asyncVariable0) {
					await smtp.SendMailAsync(message);
				}
				_sendingReport = false;
				lblInfo.Text = "Report Sent!";
				await Task.Delay(0x3e8);
				Environment.Exit(0);
				IsHitTestVisible = true;
			}
			catch {
				Environment.Exit(0);
			}
		}

		private void wndErrorReport_Closing(object sender, CancelEventArgs e) {
			if (_sendingReport) {
				e.Cancel = true;
			}
			else {
				Environment.Exit(0);
			}
		}

		public string Message { get; set; }

		public string StackTrace { get; set; }
	}
}
