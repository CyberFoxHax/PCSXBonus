using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;
using PCSX2Bonus.Properties;

namespace PCSX2Bonus.Legacy {
	internal sealed class GameManager {
		public static List<string> GameDatabase = new List<string>();

		private static List<string> _cachedDB = new List<string>();

		private static List<Game> queuedUpdates = new List<Game>();

		private static bool _isUpdating = false;

		private static HtmlDocument _doc = new HtmlDocument();

		public static HtmlWeb _web = new HtmlWeb();

		public static void AddToLibrary(Game g) {
			if (Game.AllGames.Any((Game game) => game.Serial == g.Serial)) {
				return;
			}
			Game.AllGames.Add(g);
			GameManager.AddToXml(g);
			if (Settings.Default.saveInfo) {
				GameManager.QueueUpdate(g);
			}
		}

		public static void ImportConfig(Game g) {
			string path = Path.Combine(Settings.Default.pcsx2DataDir, "inis");
			string text = Path.Combine(UserSettings.ConfigDir, g.FileSafeTitle);
			if (Directory.Exists(text)) {
				return;
			}
			Directory.CreateDirectory(text);
			string[] files = Directory.GetFiles(path, "*.ini");
			for (int i = 0; i < files.Length; i++) {
				string text2 = files[i];
				File.Copy(text2, Path.Combine(text, Path.GetFileName(text2)), true);
			}
			File.Create(Path.Combine(text, "PCSX2Bonus.ini"));
		}

		public static void QueueUpdate(Game g) {
			if (GameManager.queuedUpdates.Any((Game q) => q.Serial == g.Serial)) {
				return;
			}
			if (g.Description == "n/a*" || g.ImagePath.IsEmpty()) {
				GameManager.queuedUpdates.Add(g);
			}
			if (GameManager._isUpdating) {
				return;
			}
			GameManager.ProcessUpdateQueue();
		}

		private static async void ProcessUpdateQueue() {
			if (GameManager.queuedUpdates.Count != 0) {
				GameManager._isUpdating = true;
				await GameManager.FetchInfo(GameManager.queuedUpdates[0]);
				GameManager.GameUpdated(GameManager.queuedUpdates[0]);
			}
		}

		private static void GameUpdated(Game g) {
			GameManager.queuedUpdates.Remove(g);
			if (GameManager.queuedUpdates.Count > 0) {
				GameManager.ProcessUpdateQueue();
				return;
			}
			GameManager._isUpdating = false;
		}

		public static void Remove(Game g) {
			XElement xElement = UserSettings.xGames.Descendants("Game").FirstOrDefault((XElement x) => x.Element("Serial").Value == g.Serial);
			if (xElement != null) {
				xElement.Remove();
			}
			string text = Directory.EnumerateFiles(UserSettings.ImageDir).FirstOrDefault((string f) => Path.GetFileNameWithoutExtension(f) == g.Title.CleanFileName());
			if (!text.IsEmpty()) {
				try {
					File.Delete(text);
				}
				catch {
				}
			}
			try {
				Game.AllGames.Remove(g);
			}
			catch {
			}
		}

		public static async void UpdateGamesToLatestCompatibility() {
			if (Settings.Default.useUpdatedCompat) {
				using (WebClient webClient = new WebClient()) {
					List<string> list = new List<string>();
					try {
						string text = await webClient.DownloadStringTaskAsync("http://bositman.pcsx2.net/data/data.csv");
						string[] source = text.Split(new char[]
						{
							'\n'
						});
						Console.WriteLine("downloaded results");
						using (IEnumerator<Game> var_19 = Game.AllGames.GetEnumerator()) {
							while (var_19.MoveNext()) {
								Game g = var_19.Current;
								if (source.Any((string db) => db.Contains(g.Serial.Replace("-", "")))) {
									string text2 = source.FirstOrDefault((string db) => db.Contains(g.Serial.Replace("-", "")));
									string[] array = text2.Split(new char[]
									{
										'\t'
									});
									int num = int.Parse(array[1]);
									if (g.Compatibility != num) {
										foreach (XElement current in UserSettings.xGames.Descendants("Game")) {
											if (current.Element("Serial").Value == g.Serial) {
												int compatibility = g.Compatibility;
												current.Element("Compatibility").Value = num.ToString();
												g.Compatibility = num;
												list.Add(string.Format("{0}: {1} > {2}", g.Title, compatibility, g.Compatibility));
											}
										}
									}
								}
							}
						}
					}
					catch {
					}
					finally {
						if (list.Count != 0) {
							string messageBoxText = string.Format("The following games have been updated to their latest compatibility rating:\n{0}", string.Join("\n", list));
							MessageBox.Show(messageBoxText, "Some games have been updated", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
					}
				}
			}
		}

		public static async Task<string> FetchWideScreenPatches(Game g) {
			string result = string.Empty;
			string str = await GameManager.FetchCRC(g);
			Console.WriteLine("passed crc check");
			string address = "https://dl.dropboxusercontent.com/u/145929934/PCSX2Bonus/cheats_ws/" + str + ".pnach";
			using (WebClient webClient = new WebClient()) {
				try {
					result = await webClient.DownloadStringTaskAsync(address);
				}
				catch {
					Console.WriteLine("no patch found");
				}
			}
			return result;
		}

		public static async Task<List<string>> OnlineDatabase() {
			List<string> result = new List<string>();
			using (WebClient webClient = new WebClient()) {
				try {
					string text = await webClient.DownloadStringTaskAsync("http://bositman.pcsx2.net/data/data.csv");
					result = text.Split(new string[]
					{
						"\n"
					}, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				}
				catch {
				}
			}
			return result;
		}

		public static async Task<string> FetchCRC(Game g) {
			string result = string.Empty;
			if (GameManager._cachedDB.Count == 0) {
				GameManager._cachedDB = await GameManager.OnlineDatabase();
			}
			foreach (string current in GameManager._cachedDB) {
				string b = current.Split(new char[]
				{
					'\t'
				})[0];
				if (g.Serial.Replace("-", "") == b) {
					result = current.Split(new char[]
					{
						'\t'
					})[3];
				}
			}
			return result;
		}

		public static List<SaveState> FetchSaveStates(Game g) {
			List<SaveState> list = new List<SaveState>();
			string[] array = (from f in Directory.GetFiles(UserSettings.Pcsx2SS)
							  where Path.GetFileName(f).StartsWith(g.Serial)
							  select f).ToArray<string>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++) {
				string text = array2[i];
				FileInfo fileInfo = new FileInfo(text);
				SaveState item = new SaveState {
					Name = fileInfo.Name,
					LastModified = fileInfo.LastWriteTime,
					Type = (fileInfo.Extension == ".p2s") ? "P2S File" : "BACKUP File",
					Size = fileInfo.Length.GetSizeReadable(),
					Location = text
				};
				list.Add(item);
			}
			return list;
		}

		public static async Task<List<string>> FetchScreenshots(Game g) {
			List<string> list = new List<string>();
			string search = g.Title.CleanFileName().Replace(" ", "+");
			List<string> result;
			try {
				int num = 0;
				if (Directory.Exists(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle))) {
					string[] array = Directory.GetFiles(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle)).ToArray<string>();
					if (array.Length > 0) {
						string[] array2 = array;
						for (int i = 0; i < array2.Length; i++) {
							string item = array2[i];
							list.Add(item);
							num++;
						}
					}
					if (array.Length == 6) {
						result = list;
						return result;
					}
				}
				await Task.Run<HtmlDocument>(() => GameManager._doc = GameManager._web.Load(string.Format("https://www.google.com/search?q={0}+ign", search)));
				Console.WriteLine(search);
				HtmlNode htmlNode = (from n in GameManager._doc.DocumentNode.SelectNodes("//h3[@class='r']//a")
									 where n.Attributes["href"].Value.Contains("www.ign.com/games/") && GameData.Consoles.Any(new Func<string, bool>(n.Attributes["href"].Value.Contains)) && !n.Attributes["href"].Value.Contains("xbox-360")
									 select n).FirstOrDefault<HtmlNode>();
				if (htmlNode == null) {
					throw new Exception("Source node cannot be null");
				}
				int num2 = 0;
				string loadPage = htmlNode.Attributes["href"].Value.Between("/url?q=", "&amp").Replace("/games/", "/images/games/").Replace("/ps2", "-ps2").Replace("/xbox", "-xbox").Replace("/gcn", "-gcn");
				Console.WriteLine(loadPage);
				await Task.Run<HtmlDocument>(() => GameManager._doc = GameManager._web.Load(loadPage));
				foreach (HtmlNode current in GameManager._doc.DocumentNode.SelectNodes("//a[@class='imageGalleryThumbLink']//img")) {
					if (num2 < num) {
						num2++;
					}
					else {
						string value = current.Attributes["src"].Value;
						string item2 = value.Replace("160w", "640w");
						if (num2 >= 6) {
							break;
						}
						list.Add(item2);
						num2++;
					}
				}
			}
			catch {
				Tools.ShowMessage("Error loading screenshots", MessageType.Error);
			}
			result = list;
			return result;
		}

		public static async Task GenerateUserLibrary() {
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Game), new XmlRootAttribute("Game"));
			string[] files = Directory.GetFiles(UserSettings.ImageDir);
			foreach (XElement current in UserSettings.xGames.Descendants("Game")) {
				Game g = (Game)xmlSerializer.Deserialize(current.CreateReader());
				string text = files.FirstOrDefault((string f) => Path.GetFileNameWithoutExtension(f) == g.FileSafeTitle);
				if (g.ImagePath != "none") {
					g.ImagePath = ((!text.IsEmpty()) ? text : "");
				}
				current.Element("ImagePath").Value = g.ImagePath;
				Game.AllGames.Add(g);
				if (Settings.Default.saveInfo) {
					GameManager.QueueUpdate(g);
				}
			}
		}

		private static void AddToXml(Game g) {
			if (UserSettings.xGames.Descendants("Game").Any((XElement game) => game.Element("Serial").Value == g.Serial)) {
				return;
			}
			XElement content = new XElement("Game", new object[]
			{
				new XElement("Serial", g.Serial),
				new XElement("Name", g.Title),
				new XElement("Region", g.Region),
				new XElement("Compatibility", g.Compatibility),
				new XElement("Location", g.Location),
				new XElement("Score", g.MetacriticScore),
				new XElement("Description", g.Description),
				new XElement("Release", g.ReleaseDate),
				new XElement("Publisher", g.Publisher),
				new XElement("Time", g.TimePlayed),
				new XElement("ImagePath", g.ImagePath)
			});
			UserSettings.xGames.Add(content);
		}

		public static async Task<List<GameSearchResult>> FetchSearchResults(Game g) {
			List<GameSearchResult> list = new List<GameSearchResult>();
			List<GameSearchResult> result;
			try {
				string search = g.Title.CleanFileName().Replace(" - ", " ").Replace(" ", "+");
				search = Regex.Replace(search, "[ ]{2,}", " ");
				if (search.Contains("-[")) {
					search = search.Remove(search.IndexOf("-["));
				}
				if (search.Contains("[Disc")) {
					string oldValue = search.Substring(search.IndexOf("[Disc"));
					search = search.Replace(oldValue, "");
				}
				await Task.Run<HtmlDocument>(() => GameManager._doc = GameManager._web.Load(string.Format("http://www.gamefaqs.com/search/index.html?platform=94&game={0}&developer=&publisher=&res=1", search)));
				HtmlNodeCollection htmlNodeCollection = GameManager._doc.DocumentNode.SelectNodes("//td[@class='rtitle']");
				if (htmlNodeCollection == null) {
					throw new Exception("Game: " + g.Title + " not found");
				}
				foreach (HtmlNode current in htmlNodeCollection) {
					string text = current.InnerText.FromHtml().Trim();
					GameSearchResult item = new GameSearchResult {
						Name = text,
						Link = current.ChildNodes["a"].Attributes["href"].Value,
						Rating = LevenshteinDistance.Compute(search, text)
					};
					list.Add(item);
				}
				result = list;
			}
			catch {
				result = list;
			}
			return result;
		}

		public static async Task FetchInfo(Game g) {
			try {
				List<GameSearchResult> list = await GameManager.FetchSearchResults(g);
				if (list.Count != 0) {
					list = (from r in list
							orderby r.Rating
							select r).ToList<GameSearchResult>();
					string loadPage = "http://www.gamefaqs.com" + list[0].Link;
					await Task.Run<HtmlDocument>(() => GameManager._doc = GameManager._web.Load(loadPage));
					HtmlNode htmlNode = GameManager._doc.DocumentNode.SelectSingleNode("//div[@class='details']");
					HtmlNode htmlNode2 = GameManager._doc.DocumentNode.SelectSingleNode("//li[contains(., 'MetaCritic MetaScore')]//span");
					HtmlNode nextSibling = GameManager._doc.DocumentNode.SelectSingleNode("//li[@class='core-platform']").NextSibling;
					HtmlNode htmlNode3 = GameManager._doc.DocumentNode.SelectSingleNode("//li[contains(., 'Release:')]//a");
					HtmlNode htmlNode4 = GameManager._doc.DocumentNode.SelectSingleNode("//img[@class='boxshot']");
					string imgLocation = htmlNode4.Attributes["src"].Value.Replace("_thumb", "_front");
					g.MetacriticScore = ((htmlNode2 != null) ? htmlNode2.InnerText.Trim().FromHtml() : "n/a");
					g.Publisher = ((nextSibling != null) ? nextSibling.InnerText.Trim().FromHtml() : "n/a");
					g.ReleaseDate = ((htmlNode3 != null) ? htmlNode3.InnerText.Trim().FromHtml().Replace("»", "") : "n/a");
					g.Description = ((htmlNode != null) ? htmlNode.InnerText.Trim().FromHtml() : "n/a");
					XElement xElement = UserSettings.xGames.Descendants("Game").FirstOrDefault((XElement _g) => _g.Element("Serial").Value == g.Serial);
					if (xElement != null) {
						xElement.Element("Score").Value = g.MetacriticScore;
						xElement.Element("Publisher").Value = g.Publisher;
						xElement.Element("Release").Value = g.ReleaseDate;
						xElement.Element("Description").Value = g.Description;
						try {
							if (g.ImagePath.IsEmpty() && !imgLocation.Contains("noboxshot")) {
								if (File.Exists(Path.Combine(UserSettings.ImageDir, g.FileSafeTitle) + ".jpg")) {
									g.ImagePath = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle) + ".jpg";
									return;
								}
								string saveTo = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle + Path.GetExtension(imgLocation));
								await Task.Run(delegate {
									Tools.SaveFromWeb(imgLocation, saveTo);
								});
								g.ImagePath = saveTo;
							}
							else if (g.ImagePath.IsEmpty() && imgLocation.Contains("noboxshot")) {
								xElement.Element("ImagePath").Value = "none";
								g.ImagePath = "none";
							}
						}
						catch {
							Console.WriteLine("failed fetching image for: " + g.Title);
						}
						Console.WriteLine("fetched info for: " + g.Title);
					}
				}
			}
			catch (Exception) {
				Console.WriteLine("failed fetching info for: " + g.Title);
				XElement xElement2 = UserSettings.xGames.Descendants("Game").FirstOrDefault((XElement _g) => _g.Element("Serial").Value == g.Serial);
				if (xElement2 != null) {
					g.Description = "n/a**";
					xElement2.Element("Description").Value = "n/a**";
				}
			}
		}

		public static async Task ReFetchInfo(Game g, string link) {
			try {
				string loadPage = "http://www.gamefaqs.com" + link;
				await Task.Run<HtmlDocument>(() => GameManager._doc = GameManager._web.Load(loadPage));
				HtmlNode htmlNode = GameManager._doc.DocumentNode.SelectSingleNode("//h1[@class='page-title']");
				HtmlNode htmlNode2 = GameManager._doc.DocumentNode.SelectSingleNode("//div[@class='details']");
				HtmlNode htmlNode3 = GameManager._doc.DocumentNode.SelectSingleNode("//li[contains(., 'MetaCritic MetaScore')]//span");
				HtmlNode nextSibling = GameManager._doc.DocumentNode.SelectSingleNode("//li[@class='core-platform']").NextSibling;
				HtmlNode htmlNode4 = GameManager._doc.DocumentNode.SelectSingleNode("//li[contains(., 'Release:')]//a");
				HtmlNode htmlNode5 = GameManager._doc.DocumentNode.SelectSingleNode("//img[@class='boxshot']");
				string imgLocation = htmlNode5.Attributes["src"].Value.Replace("_thumb", "_front");
				string path = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle + Path.GetExtension(imgLocation));
				if (File.Exists(path)) {
					File.Delete(path);
				}
				g.Title = ((htmlNode != null) ? htmlNode.InnerText.Trim().FromHtml() : "n/a");
				g.MetacriticScore = ((htmlNode3 != null) ? htmlNode3.InnerText.Trim().FromHtml() : "n/a");
				g.Publisher = ((nextSibling != null) ? nextSibling.InnerText.Trim().FromHtml() : "n/a");
				g.ReleaseDate = ((htmlNode4 != null) ? htmlNode4.InnerText.Trim().FromHtml().Replace("»", "") : "n/a");
				g.Description = ((htmlNode2 != null) ? htmlNode2.InnerText.Trim().FromHtml() : "n/a");
				XElement xElement = UserSettings.xGames.Descendants("Game").FirstOrDefault((XElement _g) => _g.Element("Serial").Value == g.Serial);
				if (xElement != null) {
					xElement.Element("Name").Value = g.Title;
					xElement.Element("Score").Value = g.MetacriticScore;
					xElement.Element("Publisher").Value = g.Publisher;
					xElement.Element("Release").Value = g.ReleaseDate;
					xElement.Element("Description").Value = g.Description;
					try {
						if (!imgLocation.Contains("noboxshot") && !imgLocation.IsEmpty()) {
							string saveTo = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle + Path.GetExtension(imgLocation));
							await Task.Run(delegate {
								Tools.SaveFromWeb(imgLocation, saveTo);
							});
							g.ImagePath = saveTo;
						}
						else if (imgLocation.Contains("noboxshot")) {
							Console.WriteLine("no image found for " + g.Title);
						}
					}
					catch {
						Console.WriteLine("failed fetching image for: " + g.Title);
					}
					Console.WriteLine("fetched info for: " + g.Title);
				}
			}
			catch (Exception ex) {
				Console.WriteLine("failed fetching info for: " + g.Title);
				Console.WriteLine("Reason: " + ex.Message);
				XElement xElement2 = UserSettings.xGames.Descendants("Game").FirstOrDefault((XElement _g) => _g.Element("Serial").Value == g.Serial);
				if (xElement2 != null) {
					g.Description = "n/a**";
					xElement2.Element("Description").Value = "n/a**";
				}
			}
		}

		public static async Task<Game> GameFromImage(string file) {
			string cleanSerial = await new SerialFinder().ExtractSerial(file);
			string text = GameManager.GameDatabase.FirstOrDefault((string g) => g.Trim().Split(new char[]
			{
				'\n'
			})[0].Trim() == cleanSerial || g.Trim().Split(new char[]
			{
				'\n'
			})[0].Replace("-", "").Trim() == cleanSerial);
			text.IsEmpty();
			string[] array = text.Trim().Split(new char[]
			{
				'\n'
			});
			string serial = array[0].Trim();
			string title = array[1].Trim();
			string region = array[2].Trim();
			int num = 0;
			string text2 = array.FirstOrDefault((string s) => s.Contains("Compat"));
			if (!string.IsNullOrWhiteSpace(text2)) {
				num = (int.TryParse(text2.Replace("Compat = ", ""), out num) ? num : 0);
			}
			return new Game {
				Serial = serial,
				Title = title,
				Region = region,
				Compatibility = num,
				Location = file,
				MetacriticScore = "n/a",
				Publisher = "n/a",
				ReleaseDate = "n/a",
				Description = "n/a*",
				ImagePath = ""
			};
		}

		public static async Task AddGamesFromImages(string[] files) {
			List<string> list = new List<string>();
			string cleanSerial = string.Empty;
			int i = 0;
			while (i < files.Length) {
				try {
					cleanSerial = await new SerialFinder().ExtractSerial(files[i]);
					string text = GameManager.GameDatabase.FirstOrDefault((string g) => g.Trim().Split(new char[]
					{
						'\n'
					})[0].Trim() == cleanSerial.Trim() || g.Trim().Split(new char[]
					{
						'\n'
					})[0].Replace("-", "").Trim() == cleanSerial.Trim());
					if (text.IsEmpty()) {
						list.Add(files[i].FileNameNoExt());
						goto IL_358;
					}
					string[] array = text.Trim().Split(new char[]
					{
						'\n'
					});
					string serial = array[0].Trim();
					string title = array[1].Trim();
					string region = array[2].Trim();
					int num = 0;
					string text2 = array.FirstOrDefault((string s) => s.Contains("Compat"));
					if (!string.IsNullOrWhiteSpace(text2)) {
						num = (int.TryParse(text2.Replace("Compat = ", ""), out num) ? num : 0);
					}
					Game g2 = new Game {
						Serial = serial,
						Title = title,
						Region = region,
						Compatibility = num,
						Location = files[i],
						MetacriticScore = "n/a",
						Publisher = "n/a",
						ReleaseDate = "n/a",
						Description = "n/a*",
						ImagePath = ""
					};
					GameManager.AddToLibrary(g2);
				}
				catch {
					list.Add(files[i].FileNameNoExt());
				}
				goto IL_31D;
			IL_358:
				i++;
				continue;
			IL_31D:
				if (list.Count > 0) {
					string arg = string.Join("\n", list);
					Tools.ShowMessage(string.Format("There was an error adding the following games to the library\n{0}", arg), MessageType.Error);
				}
				list.Clear();
				goto IL_358;
			}
		}

		public static async Task BuildDatabase() {
			try {
				string text = await new StreamReader(Path.Combine(Settings.Default.pcsx2Dir, "GameIndex.dbf")).ReadToEndAsync();
				GameManager.GameDatabase = (from s in text.Substring(text.IndexOf("-- Game List")).Split(new string[]
				{
					"---------------------------------------------"
				}, StringSplitOptions.RemoveEmptyEntries)
											select s.Trim().Replace("Serial = ", "").Replace("Name   = ", "").Replace("Region = ", "")).ToList<string>();
			}
			catch (Exception ex) {
				Tools.ShowMessage(ex.Message, MessageType.Error);
			}
		}

		public static void LoadXml() {
			if (!File.Exists(UserSettings.BonusXml)) {
				string text = "<MyGames></MyGames>";
				UserSettings.xGames = XElement.Parse(text);
				UserSettings.xGames.Save(UserSettings.BonusXml);
				return;
			}
			UserSettings.xGames = XElement.Load(UserSettings.BonusXml);
		}

		public static void GenerateDirectories() {
			if (!Directory.Exists(UserSettings.RootDir)) {
				Directory.CreateDirectory(UserSettings.RootDir);
			}
			if (!Directory.Exists(UserSettings.ImageDir)) {
				Directory.CreateDirectory(UserSettings.ImageDir);
			}
			if (!Directory.Exists(UserSettings.ConfigDir)) {
				Directory.CreateDirectory(UserSettings.ConfigDir);
			}
			if (!Directory.Exists(UserSettings.ScreensDir)) {
				Directory.CreateDirectory(UserSettings.ScreensDir);
			}
			if (!Directory.Exists(UserSettings.ShadersDir)) {
				Directory.CreateDirectory(UserSettings.ShadersDir);
			}
		}
	}
}
