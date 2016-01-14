namespace PCSX2Bonus.Legacy{
	internal class GameManager{
		public static void Remove(Game game2){
		}

		public static System.Collections.Generic.List<string> GameDatabase { get; set; }

		public static void AddToLibrary(Game game){
		}

		public static System.Threading.Tasks.Task AddGamesFromImages(string[] fileNames){
			return null;
		}

		public static System.Threading.Tasks.Task ReFetchInfo(Game tag, string link){
			return null;
		}

		public static System.Threading.Tasks.Task<string> FetchCRC(Game game){
			return null;
		}

		public static System.Threading.Tasks.Task BuildDatabase(){
			return null;
		}

		public static void GenerateDirectories(){
		}

		public static void LoadXml(){
		}

		public static System.Threading.Tasks.Task GenerateUserLibrary(){
			return null;
		}

		public static void UpdateGamesToLatestCompatibility(){
		}

		public static System.Collections.Generic.List<SaveState> FetchSaveStates(Game selectedItem){
			return null;
		}

		public static System.Threading.Tasks.Task<System.Collections.Generic.List<GameSearchResult>> FetchSearchResults(Game selectedItem){
			return null;
		}

		public static System.Threading.Tasks.Task<string> FetchWideScreenPatches(Game selectedItem){
			return null;
		}

		public static System.Threading.Tasks.Task<System.Collections.Generic.List<string>> FetchScreenshots(Game tag){
			return null;
		}

		public static void ImportConfig(Game game){
		}
	}
}