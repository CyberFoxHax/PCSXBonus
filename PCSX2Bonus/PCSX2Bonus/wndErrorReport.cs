namespace PCSX2Bonus
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;

    public sealed class wndErrorReport : Window, IComponentConnector
    {
        private bool _contentLoaded;
        private string _message;
        private bool _sendingReport;
        private string _stackTrace;
        private const string body = "";
        internal Button btnOk;
        internal Button btnSend;
        private const string fromPassword = "h@te3027";
        internal TextBlock lblInfo;
        private const string subject = "Error Report";

        public wndErrorReport()
        {
            this.InitializeComponent();
            base.Closing += new CancelEventHandler(this.wndErrorReport_Closing);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this._sendingReport = true;
                this.IsHitTestVisible = false;
                this.lblInfo.Text = "Sending Report...";
                MailAddress from = new MailAddress("miketanner3128@gmail.com", "PCSX2 Tester");
                MailAddress to = new MailAddress("miketanner3128@gmail.com", "PCSX2 Developer");
                SmtpClient smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 0x24b,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(from.Address, "h@te3027"),
                    Timeout = 0x4e20
                };
                MailMessage asyncVariable0 = new MailMessage(from, to) {
                    Subject = "Error Report",
                    Body = this.Message + "\n\n\n" + this.StackTrace
                };
                using (MailMessage message = asyncVariable0)
                {
                    await smtp.SendMailAsync(message);
                }
                this._sendingReport = false;
                this.lblInfo.Text = "Report Sent!";
                await Task.Delay(0x3e8);
                Environment.Exit(0);
                this.IsHitTestVisible = true;
            }
            catch
            {
                Environment.Exit(0);
            }
        }

        [GeneratedCode("PresentationBuildTasks", "4.0.0.0"), DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/PCSX2Bonus;component/gui/wnderrorreport.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode, EditorBrowsable(EditorBrowsableState.Never), GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.btnOk = (Button) target;
                    this.btnOk.Click += new RoutedEventHandler(this.btnOk_Click);
                    return;

                case 2:
                    this.btnSend = (Button) target;
                    this.btnSend.Click += new RoutedEventHandler(this.btnSend_Click);
                    return;

                case 3:
                    this.lblInfo = (TextBlock) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private void wndErrorReport_Closing(object sender, CancelEventArgs e)
        {
            if (this._sendingReport)
            {
                e.Cancel = true;
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }

        public string StackTrace
        {
            get
            {
                return this._stackTrace;
            }
            set
            {
                this._stackTrace = value;
            }
        }

    }
}

