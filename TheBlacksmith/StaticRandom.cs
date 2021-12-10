using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith
{
    public static class StaticRandom
    {
        private static Random random { get; set; } = new Random();

        public static int Next(int min, int max)
        {
            return random.Next(min, max+1);
        }

        public static bool FlipACoin()
        {
            return Convert.ToBoolean(random.Next(0, 2));
        }
    }
}
