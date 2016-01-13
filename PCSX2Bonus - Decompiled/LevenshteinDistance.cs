using System;

internal static class LevenshteinDistance
{
    public static int Compute(string s, string t)
    {
        int length = s.Length;
        int num2 = t.Length;
        int[,] numArray = new int[length + 1, num2 + 1];
        if (length == 0)
        {
            return num2;
        }
        if (num2 == 0)
        {
            return length;
        }
        int num3 = 0;
        while (num3 <= length)
        {
            numArray[num3, 0] = num3++;
        }
        int num4 = 0;
        while (num4 <= num2)
        {
            numArray[0, num4] = num4++;
        }
        for (int i = 1; i <= length; i++)
        {
            for (int j = 1; j <= num2; j++)
            {
                int num7 = (t[j - 1] == s[i - 1]) ? 0 : 1;
                numArray[i, j] = Math.Min(Math.Min((int) (numArray[i - 1, j] + 1), (int) (numArray[i, j - 1] + 1)), numArray[i - 1, j - 1] + num7);
            }
        }
        return numArray[length, num2];
    }
}

