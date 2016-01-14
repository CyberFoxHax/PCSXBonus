using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCSX2Bonus.Legacy;

namespace PCSX2Bonus {
	internal sealed class SerialFinder {
		public async Task<string> ExtractSerial(string file) {
			string result;
			using (StreamReader streamReader = new StreamReader(file)) {
				streamReader.BaseStream.Position = 30000L;
				string s2;
				while ((s2 = streamReader.ReadLine()) != null) {
					if (streamReader.BaseStream.Position > 600000L) {
						break;
					}
					byte[] bytes = Encoding.UTF8.GetBytes(s2);
					string rawString = Encoding.UTF8.GetString(bytes);
					string text = GameData.AcceptableSerials.FirstOrDefault((string s) => rawString.Contains(s));
					if (!text.IsEmpty()) {
						string[] array = rawString.Substring(rawString.IndexOf(text)).Split(new char[]
						{
							' '
						});
						if (array[0].Length != text.Length) {
							string text2 = array[0].Replace(".", "").Replace("_", "-");
							if (text2.Contains(";")) {
								text2 = text2.Remove(text2.IndexOf(";"));
							}
							if (text2 == "SLES-5314") {
								text2 = "SLES-53142";
							}
							try {
								string text3 = text2.Substring(text2.IndexOf("-")).Replace("-", "");
								if (char.IsLetter(text3[0])) {
									continue;
								}
							}
							catch {
								Console.WriteLine("failed, string is empty");
							}
							result = text2.Trim();
							return result;
						}
					}
				}
			}
			result = string.Empty;
			return result;
		}
	}
}
