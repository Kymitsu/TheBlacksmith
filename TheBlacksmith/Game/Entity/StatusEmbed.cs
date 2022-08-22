using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class StatusEmbed : PagedMessage<EmbedBuilder>
    {
        public Player Player { get; set; }

        public StatusEmbed(Player player)
        {
            Player = player;
            BuildPages();
        }

        public override void BuildPages()
        {
            Pages.Add(1, () => {
                EmbedBuilder page1 = new EmbedBuilder();
                page1.WithAuthor(Player.Name);
                page1.WithTitle("Status");
                page1.WithDescription($"Lvl: {Player.Lvl}{Environment.NewLine}{Utility.BuildProgressBar(Player.CurrentExp, Player.ExpToLvl, 20)}");

                page1.AddField($"HP: {Player.CurrentHp}/{Player.Hp}", Utility.BuildProgressBar(Player.CurrentHp, Player.Hp, 10));

                //page1.WithFooter("page 1/x");

                return page1;
            });

            Pages.Add(2, () => {
                EmbedBuilder page2 = new EmbedBuilder();
                page2.WithAuthor(Player.Name);
                page2.WithTitle("Skills");

                foreach (var item in Player.Attacks)
                {
                    page2.AddField(item.Method.Name, "No description", true);
                }

                return page2;
            });

        }
    }
}
