using System;
using System.Drawing;

namespace RunGet
{
    public class Utils
    {
        public static Color GetColorFromRank(int rank)
        {
            return rank switch
            {
                // Gold, Silver, and Bronze
                1 => Color.FromArgb(201, 176, 55),
                2 => Color.FromArgb(215, 215, 215),
                3 => Color.FromArgb(106, 56, 5),

                // Default color (Blue)
                _ => Color.FromArgb(42, 137, 231),
            };
        }

        public static void Seperator(int length = 112, char c = '=')
        {
            Console.WriteLine("".PadRight(length, c));
        }
    }
}
