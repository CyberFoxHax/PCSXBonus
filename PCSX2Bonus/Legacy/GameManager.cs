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
		private static List<string> _cachedDb = new List<string>();
		private static readonly List<Game> QueuedUpdates = new List<Game>();
		private static bool _isUpdating;
		private static HtmlDocument _doc = new HtmlDocument();
		public static HtmlWeb Web = new HtmlWeb();

		public static void AddToLibrary(Game g) {
			if (Game.AllGames.Any(game => game.Serial == g.Serial))
				return;
			Game.AllGames.Add(g);
			AddToXml(g);
			if (Settings.Default.saveInfo)
				QueueUpdate(g);
		}

		public static void ImportConfig(Game g) {
			var path = Path.Combine(Settings.Default.pcsx2DataDir, "inis");
			var text = Path.Combine(UserSettings.ConfigDir, g.FileSafeTitle);
			if (Directory.Exists(text))
				return;
			Directory.CreateDirectory(text);
			var files = Directory.GetFiles(path, "*.ini");
			foreach (var text2 in files)
				File.Copy(text2, Path.Combine(text, Path.GetFileName(text2)), true);
			File.Create(Path.Combine(text, "PCSX2Bonus.ini"));
		}

		public static void QueueUpdate(Game g) {
			if (QueuedUpdates.Any(q => q.Serial == g.Serial))
				return;
			if (g.Description == "n/a*" || g.ImagePath.IsEmpty())
				QueuedUpdates.Add(g);
			if (_isUpdating)
				return;
			ProcessUpdateQueue();
		}

		private static async void ProcessUpdateQueue() {
			if (QueuedUpdates.Count == 0) return;
			_isUpdating = true;
			await FetchInfo(QueuedUpdates[0]);
			GameUpdated(QueuedUpdates[0]);
		}

		private static void GameUpdated(Game g) {
			QueuedUpdates.Remove(g);
			if (QueuedUpdates.Count > 0) {
				ProcessUpdateQueue();
				return;
			}
			_isUpdating = false;
		}

		public static void Remove(Game g) {
			var xElement = UserSettings.xGames.Descendants("Game").FirstOrDefault(x => x.Element("Serial").Value == g.Serial);
			if (xElement != null)
				xElement.Remove();
			var text = Directory.EnumerateFiles(UserSettings.ImageDir).FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == g.Title.CleanFileName());
			if (!text.IsEmpty())
				try{
					File.Delete(text);
				}
				catch{
				}
			try {
				Game.AllGames.Remove(g);
			}
			catch {
			}
		}

		public static async void UpdateGamesToLatestCompatibility() {
			if (Settings.Default.useUpdatedCompat) {
				using (var webClient = new WebClient()) {
					var list = new List<string>();
					try {
						var text = await webClient.DownloadStringTaskAsync("http://bositman.pcsx2.net/data/data.csv");
						var source = text.Split('\n');
						Console.WriteLine("downloaded results");
						using (var var_19 = Game.AllGames.GetEnumerator()) {
							while (var_19.MoveNext()) {
								var g = var_19.Current;
								if (source.Any(db => db.Contains(g.Serial.Replace("-", "")))) {
									var text2 = source.FirstOrDefault(db => db.Contains(g.Serial.Replace("-", "")));
									var array = text2.Split('\t');
									var num = int.Parse(array[1]);
									if (g.Compatibility != num) {
										foreach (var current in UserSettings.xGames.Descendants("Game")) {
											if (current.Element("Serial").Value == g.Serial) {
												var compatibility = g.Compatibility;
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
							var messageBoxText = string.Format("The following games have been updated to their latest compatibility rating:\n{0}", string.Join("\n", list));
							MessageBox.Show(messageBoxText, "Some games have been updated", MessageBoxButton.OK, MessageBoxImage.Asterisk);
						}
					}
				}
			}
		}

		public static async Task<string> FetchWideScreenPatches(Game g) {
			var result = string.Empty;
			var str = await FetchCRC(g);
			Console.WriteLine("passed crc check");
			var address = "https://dl.dropboxusercontent.com/u/145929934/PCSX2Bonus/cheats_ws/" + str + ".pnach";
			using (var webClient = new WebClient())
				try{
					result = await webClient.DownloadStringTaskAsync(address);
				}
				catch{
					Console.WriteLine("no patch found");
				}
			return result;
		}

		public static async Task<List<string>> OnlineDatabase() {
			var result = new List<string>();
			using (var webClient = new WebClient())
				try{
					var text = await webClient.DownloadStringTaskAsync("http://bositman.pcsx2.net/data/data.csv");
					result = text.Split(new[]{
						"\n"
					}, StringSplitOptions.RemoveEmptyEntries).ToList();
				}
				catch{
				}
			return result;
		}

		public static async Task<string> FetchCRC(Game g) {
			var result = string.Empty;
			if (_cachedDb.Count == 0)
				_cachedDb = await OnlineDatabase();
			foreach (var current in
				from current in _cachedDb
				let b = current.Split('\t')[0]
				where g.Serial.Replace("-", "") == b
				select current
			)
				result = current.Split('\t')[3];
			return result;
		}

		public static List<SaveState> FetchSaveStates(Game g) {
			var array = (
				from f in Directory.GetFiles(UserSettings.Pcsx2SS)
				where Path.GetFileName(f).StartsWith(g.Serial)
				select f
			).ToArray();
			var array2 = array;
			return (
				from text in array2
				let fileInfo = new FileInfo(text)
				select new SaveState{
					Name = fileInfo.Name,
					LastModified = fileInfo.LastWriteTime,
					Type = (fileInfo.Extension == ".p2s") ? "P2S File" : "BACKUP File",
					Size = fileInfo.Length.GetSizeReadable(),
					Location = text
				}
			).ToList();
		}

		public static async Task<List<string>> FetchScreenshots(Game g) {
			var list = new List<string>();
			var search = g.Title.CleanFileName().Replace(" ", "+");
			List<string> result;
			try {
				var num = 0;
				if (Directory.Exists(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle))) {
					var array = Directory.GetFiles(Path.Combine(UserSettings.ScreensDir, g.FileSafeTitle)).ToArray();
					if (array.Length > 0){
						var array2 = array;
						foreach (var item in array2){
							list.Add(item);
							num++;
						}
					}
					if (array.Length == 6) {
						result = list;
						return result;
					}
				}
				await Task.Run(() => _doc = Web.Load(string.Format("https://www.google.com/search?q={0}+ign", search)));
				Console.WriteLine(search);
				var htmlNode = (
					from n in _doc.DocumentNode.SelectNodes("//h3[@class='r']//a")
					where n.Attributes["href"].Value.Contains("www.ign.com/games/")
					&& GameData.Consoles.Any(n.Attributes["href"].Value.Contains)
					&& n.Attributes["href"].Value.Contains("xbox-360") == false
					select n
				).FirstOrDefault<HtmlNode>();
				if (htmlNode == null)
					throw new Exception("Source node cannot be null");
				var num2 = 0;
				var loadPage = htmlNode.Attributes["href"].Value.Between("/url?q=", "&amp").Replace("/games/", "/images/games/").Replace("/ps2", "-ps2").Replace("/xbox", "-xbox").Replace("/gcn", "-gcn");
				Console.WriteLine(loadPage);
				await Task.Run(() => _doc = Web.Load(loadPage));
				foreach (var current in _doc.DocumentNode.SelectNodes("//a[@class='imageGalleryThumbLink']//img")) {
					if (num2 < num)
						num2++;
					else{
						var value = current.Attributes["src"].Value;
						var item2 = value.Replace("160w", "640w");
						if (num2 >= 6)
							break;
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
			var xmlSerializer = new XmlSerializer(typeof(Game), new XmlRootAttribute("Game"));
			var files = Directory.GetFiles(UserSettings.ImageDir);
			foreach (var current in UserSettings.xGames.Descendants("Game")) {
				var g = (Game)xmlSerializer.Deserialize(current.CreateReader());
				var text = files.FirstOrDefault(f => Path.GetFileNameWithoutExtension(f) == g.FileSafeTitle);
				if (g.ImagePath != "none")
					g.ImagePath = text.IsEmpty() ? "" : text;
				current.Element("ImagePath").Value = g.ImagePath;
				Game.AllGames.Add(g);
				if (Settings.Default.saveInfo)
					QueueUpdate(g);
			}
		}

		private static void AddToXml(Game g) {
			if (UserSettings.xGames.Descendants("Game").Any(game => game.Element("Serial").Value == g.Serial))
				return;
			var content = new XElement("Game", new object[]{
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
			var list = new List<GameSearchResult>();
			List<GameSearchResult> result;
			try {
				var search = g.Title.CleanFileName().Replace(" - ", " ").Replace(" ", "+");
				search = Regex.Replace(search, "[ ]{2,}", " ");
				if (search.Contains("-["))
					search = search.Remove(search.IndexOf("-["));
				if (search.Contains("[Disc")) {
					var oldValue = search.Substring(search.IndexOf("[Disc"));
					search = search.Replace(oldValue, "");
				}
				await Task.Run(() => _doc = Web.Load(string.Format("http://www.gamefaqs.com/search/index.html?platform=94&game={0}&developer=&publisher=&res=1", search)));
				var htmlNodeCollection = _doc.DocumentNode.SelectNodes("//td[@class='rtitle']");
				if (htmlNodeCollection == null)
					throw new Exception("Game: " + g.Title + " not found");
				list.AddRange(from current in htmlNodeCollection
					let text = current.InnerText.FromHtml().Trim()
					select new GameSearchResult{
						Name = text, 
						Link = current.ChildNodes["a"].Attributes["href"].Value,
						Rating = LevenshteinDistance.Compute(search, text)
					});
				result = list;
			}
			catch {
				result = list;
			}
			return result;
		}

		public static async Task FetchInfo(Game g) {
			try {
				var list = await FetchSearchResults(g);
				if (list.Count != 0) {
					list = (from r in list
							orderby r.Rating
							select r).ToList<GameSearchResult>();
					var loadPage = "http://www.gamefaqs.com" + list[0].Link;
					await Task.Run(() => _doc = Web.Load(loadPage));
					var htmlNode = _doc.DocumentNode.SelectSingleNode("//div[@class='details']");
					var htmlNode2 = _doc.DocumentNode.SelectSingleNode("//li[contains(., 'MetaCritic MetaScore')]//span");
					var nextSibling = _doc.DocumentNode.SelectSingleNode("//li[@class='core-platform']").NextSibling;
					var htmlNode3 = _doc.DocumentNode.SelectSingleNode("//li[contains(., 'Release:')]//a");
					var htmlNode4 = _doc.DocumentNode.SelectSingleNode("//img[@class='boxshot']");
					var imgLocation = htmlNode4.Attributes["src"].Value.Replace("_thumb", "_front");
					g.MetacriticScore = ((htmlNode2 != null) ? htmlNode2.InnerText.Trim().FromHtml() : "n/a");
					g.Publisher = ((nextSibling != null) ? nextSibling.InnerText.Trim().FromHtml() : "n/a");
					g.ReleaseDate = ((htmlNode3 != null) ? htmlNode3.InnerText.Trim().FromHtml().Replace("»", "") : "n/a");
					g.Description = ((htmlNode != null) ? htmlNode.InnerText.Trim().FromHtml() : "n/a");
					var xElement = UserSettings.xGames.Descendants("Game").FirstOrDefault(_g => _g.Element("Serial").Value == g.Serial);
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
								var saveTo = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle + Path.GetExtension(imgLocation));
								await Task.Run(() => Tools.SaveFromWeb(imgLocation, saveTo));
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
				var xElement2 = UserSettings.xGames.Descendants("Game").FirstOrDefault(_g => _g.Element("Serial").Value == g.Serial);
				if (xElement2 != null) {
					g.Description = "n/a**";
					xElement2.Element("Description").Value = "n/a**";
				}
			}
		}

		public static async Task ReFetchInfo(Game g, string link) {
			try {
				var loadPage = "http://www.gamefaqs.com" + link;
				await Task.Run(() => _doc = Web.Load(loadPage));
				var htmlNode = _doc.DocumentNode.SelectSingleNode("//h1[@class='page-title']");
				var htmlNode2 = _doc.DocumentNode.SelectSingleNode("//div[@class='details']");
				var htmlNode3 = _doc.DocumentNode.SelectSingleNode("//li[contains(., 'MetaCritic MetaScore')]//span");
				var nextSibling = _doc.DocumentNode.SelectSingleNode("//li[@class='core-platform']").NextSibling;
				var htmlNode4 = _doc.DocumentNode.SelectSingleNode("//li[contains(., 'Release:')]//a");
				var htmlNode5 = _doc.DocumentNode.SelectSingleNode("//img[@class='boxshot']");
				var imgLocation = htmlNode5.Attributes["src"].Value.Replace("_thumb", "_front");
				var path = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle + Path.GetExtension(imgLocation));
				if (File.Exists(path))
					File.Delete(path);
				g.Title = ((htmlNode != null) ? htmlNode.InnerText.Trim().FromHtml() : "n/a");
				g.MetacriticScore = ((htmlNode3 != null) ? htmlNode3.InnerText.Trim().FromHtml() : "n/a");
				g.Publisher = ((nextSibling != null) ? nextSibling.InnerText.Trim().FromHtml() : "n/a");
				g.ReleaseDate = ((htmlNode4 != null) ? htmlNode4.InnerText.Trim().FromHtml().Replace("»", "") : "n/a");
				g.Description = ((htmlNode2 != null) ? htmlNode2.InnerText.Trim().FromHtml() : "n/a");
				var xElement = UserSettings.xGames.Descendants("Game").FirstOrDefault((XElement _g) => _g.Element("Serial").Value == g.Serial);
				if (xElement != null) {
					xElement.Element("Name").Value = g.Title;
					xElement.Element("Score").Value = g.MetacriticScore;
					xElement.Element("Publisher").Value = g.Publisher;
					xElement.Element("Release").Value = g.ReleaseDate;
					xElement.Element("Description").Value = g.Description;
					try {
						if (!imgLocation.Contains("noboxshot") && !imgLocation.IsEmpty()) {
							var saveTo = Path.Combine(UserSettings.ImageDir, g.FileSafeTitle + Path.GetExtension(imgLocation));
							await Task.Run(() => Tools.SaveFromWeb(imgLocation, saveTo));
							g.ImagePath = saveTo;
						}
						else if (imgLocation.Contains("noboxshot"))
							Console.WriteLine("no image found for " + g.Title);
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
				var xElement2 = UserSettings.xGames.Descendants("Game").FirstOrDefault(_g => _g.Element("Serial").Value == g.Serial);
				if (xElement2 != null) {
					g.Description = "n/a**";
					xElement2.Element("Description").Value = "n/a**";
				}
			}
		}

		public static async Task<Game> GameFromImage(string file) {
			var cleanSerial = await new SerialFinder().ExtractSerial(file);
			var text = GameDatabase.FirstOrDefault(g => 
				   g
					.Trim()
					.Split('\n')[0]
					.Trim() == cleanSerial
				|| g
					.Trim()
					.Split('\n')[0]
					.Replace("-", "").Trim() == cleanSerial
				);
			text.IsEmpty();
			var array = text.Trim().Split('\n');
			var serial = array[0].Trim();
			var title = array[1].Trim();
			var region = array[2].Trim();
			var num = 0;
			var text2 = array.FirstOrDefault(s => s.Contains("Compat"));
			if (!string.IsNullOrWhiteSpace(text2))
				num = (int.TryParse(text2.Replace("Compat = ", ""), out num) ? num : 0);
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
			var list = new List<string>();
			var i = 0;
			while (i < files.Length) {
				try {
					var cleanSerial = await new SerialFinder().ExtractSerial(files[i]);
					var text = GameDatabase
						.FirstOrDefault(g => 
						   g
							.Trim()
							.Split('\n')[0]
							.Trim() == cleanSerial.Trim()
						|| g
							.Trim()
							.Split('\n')[0]
							.Replace("-", "")
							.Trim() == cleanSerial.Trim()
						);
					if (text.IsEmpty()) {
						list.Add(files[i].FileNameNoExt());
						goto IL_358;
					}
					var array = text.Trim().Split('\n');
					var serial = array[0].Trim();
					var title = array[1].Trim();
					var region = array[2].Trim();
					var num = 0;
					var text2 = array.FirstOrDefault(s => s.Contains("Compat"));
					if (!string.IsNullOrWhiteSpace(text2))
						num = (int.TryParse(text2.Replace("Compat = ", ""), out num) ? num : 0);
					var g2 = new Game {
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
					AddToLibrary(g2);
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
					var arg = string.Join("\n", list);
					Tools.ShowMessage(string.Format("There was an error adding the following games to the library\n{0}", arg), MessageType.Error);
				}
				list.Clear();
				goto IL_358;
			}
		}

		public static async Task BuildDatabase() {
			var text = await new StreamReader(Path.Combine(Settings.Default.pcsx2Dir, "GameIndex.dbf")).ReadToEndAsync();
			GameDatabase = (
				from s in text
					.Substring(text.IndexOf("-- Game List"))
					.Split(new []{"---------------------------------------------"}, StringSplitOptions.RemoveEmptyEntries)
				select s
					.Trim()
					.Replace("Serial = ", "")
					.Replace("Name   = ", "")
					.Replace("Region = ", "")
			).ToList<string>();
		}

		public static void LoadXml() {
			if (!File.Exists(UserSettings.BonusXml)) {
				const string text = "<MyGames></MyGames>";
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
