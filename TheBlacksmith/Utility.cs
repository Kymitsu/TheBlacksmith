using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith
{
    public static class Utility
    {
        public static string BuildProgressBar(int currentValue, int maxValue, int size)
        {
            //string test = "▱▰";
            string result = string.Empty;

            double temp = (currentValue / (double)maxValue) * size;

            int progressValue = (int)Math.Round(temp, MidpointRounding.AwayFromZero);

            for (int i = 0; i < size; i++)
            {
                if(i<progressValue)
                    result += '▰';
                else
                    result += '▱';
            }

            return result;
        }
    }
}
