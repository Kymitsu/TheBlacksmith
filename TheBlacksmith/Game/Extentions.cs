using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace TheBlacksmith.Game
{
    public static class Extentions
    {
        public static Player FindByMention(this List<Player> players, string mention)
        {
            return players.FirstOrDefault(x => x.Mention == mention);
        }

        public static void CopyTo(this List<String> source, List<string> target)
        {
            target.Clear();

            foreach (string item in source)
            {
                target.Add(item);
            }
        }
    }
}
