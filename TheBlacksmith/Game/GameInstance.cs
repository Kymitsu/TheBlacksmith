using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheBlacksmith.Game
{
    public static class GameInstance
    {
        public static ulong ChannelCategory = 841793104603643924;
        public static List<Player> Players { get; set; } = new List<Player>();
        public static List<Adventure> OnGoingAdventures { get; set; } = new List<Adventure>();
        public static List<Adventure> CompletedAdventures { get; set; } = new List<Adventure>();


        public static Player CreateNewPlayer(string mention, string name)
        {
            Player p = Player.CreateNewPlayer(mention, name);
            Players.Add(p);
            return p;
        }
        public static Adventure CreateNewAdventure(ulong channelId, int advLvl, Player player)
        {
            Adventure newAdventure = new Adventure(channelId, advLvl, player);
            OnGoingAdventures.Add(newAdventure);
            newAdventure.OnEndOfAdventure += OnEndOfAdventure;

            return newAdventure;
        }

        public static Adventure ContinueAdventure(Player player, string action)
        {
            Adventure adv = OnGoingAdventures.FirstOrDefault(a => a.Player == player);
            adv.NextStep(action);

            return adv;
        }

        public static void OnEndOfAdventure(object sender, EventArgs e)
        {
            var adv = (Adventure)sender;
            adv.PossibleActions.Clear();
            adv.OnEndOfAdventure -= OnEndOfAdventure;
            OnGoingAdventures.Remove(adv);
            CompletedAdventures.Add(adv);

            adv.Player.Heal(true);

        }
    }
}
