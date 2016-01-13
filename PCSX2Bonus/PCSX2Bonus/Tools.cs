namespace PCSX2Bonus
{
    using PCSX2Bonus.Properties;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Xml.Linq;

    internal sealed class Tools
    {
        private static Regex _invalidXMLChars = new Regex(@"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]", RegexOptions.Compiled);

        public static FrameworkElement FindByName(string name, FrameworkElement root)
        {
            Stack<FrameworkElement> stack = new Stack<FrameworkElement>();
            stack.Push(root);
            while (stack.Count > 0)
            {
                FrameworkElement reference = stack.Pop();
                if (reference.Name == name)
                {
                    return reference;
                }
                int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(reference, i);
                    if (child is FrameworkElement)
                    {
                        stack.Push((FrameworkElement) child);
                    }
                }
            }
            return null;
        }

        public static Visual GetDescendantByName(FrameworkElement element, string name)
        {
            if (element == null)
            {
                return null;
            }
            if (element.Name == name)
            {
                return element;
            }
            Visual descendantByName = null;
            if (element != null)
            {
                element.ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual child = VisualTreeHelper.GetChild(element, i) as Visual;
                descendantByName = GetDescendantByName((FrameworkElement) child, name);
                if (descendantByName != null)
                {
                    return descendantByName;
                }
            }
            return descendantByName;
        }

        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual descendantByType = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual child = VisualTreeHelper.GetChild(element, i) as Visual;
                descendantByType = GetDescendantByType(child, type);
                if (descendantByType != null)
                {
                    return descendantByType;
                }
            }
            return descendantByType;
        }

        public static BitmapImage GetLocalImage(string name)
        {
            return new BitmapImage(new Uri("Images/" + name, UriKind.Relative));
        }

        public static string GetSizeReadable2(long i)
        {
            string str2;
            string str = (i < 0L) ? "-" : "";
            double num = (i < 0L) ? ((double) -i) : ((double) i);
            if (i >= 0x1000000000000000L)
            {
                str2 = "EB";
                num = i >> 50;
            }
            else if (i >= 0x4000000000000L)
            {
                str2 = "PB";
                num = i >> 40;
            }
            else if (i >= 0x10000000000L)
            {
                str2 = "TB";
                num = i >> 30;
            }
            else if (i >= 0x40000000L)
            {
                str2 = "GB";
                num = i >> 20;
            }
            else if (i >= 0x100000L)
            {
                str2 = "MB";
                num = i >> 10;
            }
            else if (i >= 0x400L)
            {
                str2 = "KB";
                num = i;
            }
            else
            {
                return i.ToString(str + "0 B");
            }
            num /= 1024.0;
            return (str + num.ToString("0 ") + str2);
        }

        public static void HideAllWindows()
        {
            foreach (object obj2 in Application.Current.Windows)
            {
                ((Window) obj2).Hide();
            }
        }

        public async static Task<BitmapImage> ImageFromWeb(string url, CancellationToken ct)
        {
            BitmapImage bmi;
            WebRequest request;
            WebResponse response;
            if (!url.IsEmpty())
            {
                bmi = new BitmapImage();
                request = WebRequest.Create(url);
                request.Proxy = null;
                response = null;
            }
            else
            {
                return null;
            }
            await Task.Run<WebResponse>((Func<WebResponse>) (() => (response = request.GetResponse())), ct);
            Stream responseStream = response.GetResponseStream();
            bmi.BeginInit();
            bmi.StreamSource = responseStream;
            bmi.EndInit();
            return bmi;
        }

        public async static Task<bool> InternetAvaliable()
        {
            Ping ping = new Ping();
            try
            {
                await ping.SendPingAsync("192.168.1.1.2");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string RemoveInvalidXMLChars(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return "";
            }
            return _invalidXMLChars.Replace(text, "");
        }

        public static void SaveFromWeb(string url, string path)
        {
            if (!url.IsEmpty())
            {
                WebRequest request = WebRequest.Create(url);
                request.Proxy = null;
                Stream responseStream = request.GetResponse().GetResponseStream();
                Bitmap bitmap = new Bitmap(responseStream);
                responseStream.Dispose();
                bitmap.Save(path);
                bitmap.Dispose();
            }
        }

        public static void ShowAllWindows()
        {
            foreach (object obj2 in Application.Current.Windows)
            {
                ((Window) obj2).Show();
            }
        }

        public static void ShowMessage(string message, MessageType type)
        {
            switch (type)
            {
                case MessageType.Error:
                    MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
                    return;

                case MessageType.Info:
                    MessageBox.Show(message, "PCSX2Bonus", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    return;
            }
        }

        public static XElement TryLoad(string path)
        {
            try
            {
                return XElement.Load(path);
            }
            catch
            {
                if (System.IO.File.Exists(Path.Combine(UserSettings.ThemesDir, "default.xml")))
                {
                    Settings.Default.defaultTheme = "default.xml";
                    return XElement.Load(Path.Combine(UserSettings.ThemesDir, "default.xml"));
                }
                return new XElement("invalid");
            }
        }


    }
}

