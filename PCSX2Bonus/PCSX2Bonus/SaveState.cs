namespace PCSX2Bonus
{
    using System;

	internal sealed class SaveState
    {
        public DateTime LastModified { get; set; }

        public string Location { get; set; }

        public string Name { get; set; }

        public string Size { get; set; }

        public string Type { get; set; }
    }
}

