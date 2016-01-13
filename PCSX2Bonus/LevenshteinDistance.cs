using System;

namespace PCSX2Bonus {
	internal static class LevenshteinDistance {
		public static int Compute(string s, string t) {
			var length = s.Length;
			var num2 = t.Length;
			var numArray = new int[length + 1, num2 + 1];

			if (length == 0) return num2;
			if (num2 == 0) return length;
			var num3 = 0;
			while (num3 <= length)
				numArray[num3, 0] = num3++;
			var num4 = 0;
			while (num4 <= num2)
				numArray[0, num4] = num4++;
			for (var i = 1; i <= length; i++)
				for (var j = 1; j <= num2; j++) {
					var num7 = (t[j - 1] == s[i - 1]) ? 0 : 1;
					numArray[i, j] = Math.Min(Math.Min(numArray[i - 1, j] + 1, numArray[i, j - 1] + 1), numArray[i - 1, j - 1] + num7);
				}
			return numArray[length, num2];
		}
	}
}

