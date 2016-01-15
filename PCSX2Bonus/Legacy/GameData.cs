namespace PCSX2Bonus.Legacy {
	internal sealed class GameData {
		public static readonly string[] AcceptableFormats = {
			".ISO", ".MDF", ".BIN", ".NRG", ".IMG"
		};
		public static readonly string[] AcceptableSerials = { 
			"SCUS", "SLUS", "PCPX", "SCAJ", "SCKA", "SCPS", "SLAJ", "SLKA", "SLPM", "SLPS", "TCPS", "PBPS", "SCED", "SCES", "SLED", "SLES", "TCES"
		};
		public static readonly string[] Consoles = {
			"gcn", "xbox", "ps2"
		};
	}
}

