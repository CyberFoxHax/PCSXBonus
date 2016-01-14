namespace PCSX2Bonus.Legacy {
	public sealed class GameSearchResult {
		public string InfoString {
			get {
				return string.Format("{0} [{1}]", Name, Rating);
			}
		}

		public string Link { get; set; }
		public string Name { get; set; }
		public int Rating { get; set; }
	}
}

