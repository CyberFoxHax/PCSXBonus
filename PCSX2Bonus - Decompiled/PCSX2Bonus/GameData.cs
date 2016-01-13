namespace PCSX2Bonus
{
    using System;

    internal sealed class GameData
    {
        public static readonly string[] AcceptableFormats = new string[] { ".ISO", ".MDF", ".BIN", ".NRG", ".IMG" };
        public static readonly string[] AcceptableSerials = new string[] { 
            "SCUS", "SLUS", "PCPX", "SCAJ", "SCKA", "SCPS", "SLAJ", "SLKA", "SLPM", "SLPS", "TCPS", "PBPS", "SCED", "SCES", "SLED", "SLES", 
            "TCES"
         };
        public static readonly string[] Consoles = new string[] { "gcn", "xbox", "ps2" };
    }
}

