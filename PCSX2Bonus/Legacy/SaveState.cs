using System;

namespace PCSX2Bonus.Legacy {
	internal sealed class SaveState {
		public DateTime LastModified { get; set; }
		public string Location { get; set; }
		public string Name { get; set; }
		public string Size { get; set; }
		public string Type { get; set; }
	}
}

