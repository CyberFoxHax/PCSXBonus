using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace PCSX2Bonus.Legacy {
	public static class Extensions {
		public static Task BeginAsync(this Storyboard storyboard) {
			var tcs = new TaskCompletionSource<bool>();
			if (storyboard == null) {
				tcs.SetException(new ArgumentNullException());
			}
			else {
				EventHandler onComplete = null;
				onComplete = delegate(object s, EventArgs e) {
					storyboard.Completed -= onComplete;
					tcs.SetResult(true);
				};
				storyboard.Completed += onComplete;
				storyboard.Begin();
			}
			return tcs.Task;
		}

		public static string Between(this string src, string findfrom, string findto) {
			var index = src.IndexOf(findfrom);
			var num2 = src.IndexOf(findto, index + findfrom.Length);
			if ((index >= 0) && (num2 >= 0)) {
				return src.Substring(index + findfrom.Length, (num2 - index) - findfrom.Length);
			}
			return "";
		}

		public static string CleanFileName(this string fileName) {
			return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
		}

		public static bool Contains(this string source, string toCheck, StringComparison comp) {
			return (source.IndexOf(toCheck, comp) >= 0);
		}

		public static string Escape(this string src) {
			return src.Replace(@"\", @"\\");
		}

		public static string FileNameNoExt(this string src) {
			return Path.GetFileNameWithoutExtension(src);
		}

		public static string FromHtml(this string src) {
			return WebUtility.HtmlDecode(src);
		}

		public static string GetSizeReadable(this long i) {
			string str2;
			var str = (i < 0L) ? "-" : "";
			var num = (i < 0L) ? ((double)-i) : ((double)i);
			if (i >= 0x1000000000000000L) {
				str2 = "EB";
				num = i >> 50;
			}
			else if (i >= 0x4000000000000L) {
				str2 = "PB";
				num = i >> 40;
			}
			else if (i >= 0x10000000000L) {
				str2 = "TB";
				num = i >> 30;
			}
			else if (i >= 0x40000000L) {
				str2 = "GB";
				num = i >> 20;
			}
			else if (i >= 0x100000L) {
				str2 = "MB";
				num = i >> 10;
			}
			else if (i >= 0x400L) {
				str2 = "KB";
				num = i;
			}
			else {
				return i.ToString(str + "0 B");
			}
			num /= 1024.0;
			return (str + num.ToString("0.## ") + str2);
		}

		public static bool IsEmpty(this string src) {
			return string.IsNullOrWhiteSpace(src);
		}

		public static string Truncate(this string value, int maxLength) {
			if (value.Length > maxLength) {
				return (value.Substring(0, maxLength - 3) + "...");
			}
			return value;
		}

		public static string Unescape(this string src) {
			return src.Replace(@"\\", @"\");
		}
	}
}

