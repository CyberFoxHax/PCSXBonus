using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;

namespace PCSX2Bonus.Views {
	public sealed partial class wndErrorReport {
		private bool _sendingReport;

		public wndErrorReport(Exception exceptionObject, Window owner) {
			InitializeComponent();
			Message = exceptionObject.Message;
			StackTrace = exceptionObject.StackTrace;
			Closing += wndErrorReport_Closing;
			Owner = owner;
		}

		private void btnOk_Click(object sender, RoutedEventArgs e) {
			Environment.Exit(0);
		}

		private async void btnSend_Click(object sender, RoutedEventArgs e){
			try {
				_sendingReport = true;
				IsHitTestVisible = false;
				lblInfo.Visibility = Visibility.Visible;
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
			}
			catch {
				Environment.Exit(0);
			}
		}

		private void wndErrorReport_Closing(object sender, CancelEventArgs e) {
			if (_sendingReport)
				e.Cancel = true;
			else
				Environment.Exit(0);
		}

		public string Message{
			get { return tbMessage.Text; }
			set { tbMessage.Text = value; }
		}

		public string StackTrace{
			get { return tbStackTrace.Text; }
			set { tbStackTrace.Text = value; }
		}
	}
}
