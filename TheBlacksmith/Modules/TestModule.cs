using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheBlacksmith.Game;

namespace TheBlacksmith.Modules
{
    public class TestModule : ModuleBase
    {
        private static Random random = new Random();

        [Command("/ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync("Pong");
        }

        [Command("/newPlayer")]
        public async Task NewPlayer(string name)
        {
            _ = Context.Message.DeleteAsync();
            Player p = GameInstance.Players.FindByMention(Context.User.Mention);
            if (p == null)
            {
                p = GameInstance.CreateNewPlayer(Context.Message.Author.Mention, name);
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} Character {p.Name} - LVL{p.Lvl} created");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} You already have a character({p.Name} - LVL{p.Lvl})");
            }
        }

        [Command("/newAdventure"), Alias("/newAdv")]
        public async Task NewAdventure(int advLvl)
        {
            _ = Context.Message.DeleteAsync();
            Player player = GameInstance.Players.FindByMention(Context.Message.Author.Mention);

            var channel = await Context.Guild.CreateTextChannelAsync(
                $"{player.Name}-adventure",
                (x) => {
                    x.CategoryId = GameInstance.ChannelCategory;
                    //TODO add permission to user
                }
            );
            Adventure adv = GameInstance.CreateNewAdventure(channel.Id, advLvl, player);
            await channel.SendMessageAsync($"{player.Mention} Welcome {player.Name}! And good luck on your adventure.");


            var encounterMsg = await channel.SendMessageAsync($"```diff{Environment.NewLine}{adv.EncounterMsg.ToString()}```");
            adv.EncouterMsgID = encounterMsg.Id;

            var actionMsg = await channel.SendMessageAsync($"Vos actions /adv [] : {string.Join(" - ", adv.PossibleActions)}");
            adv.ActionsMsgID = actionMsg.Id;
        }

        [Command("/adventure"), Alias("/adv")]
        public async Task Adventure(string action)
        {
            _ = Context.Message.DeleteAsync();
            //TODO rajouter check channel id
            Player player = GameInstance.Players.FindByMention(Context.Message.Author.Mention);

            Adventure test = GameInstance.ContinueAdventure(player, action);

            await Context.Channel.ModifyMessageAsync(
                test.EncouterMsgID, 
                x => { 
                    x.Content = $"```diff{Environment.NewLine}{test.EncounterMsg.ToString()}```"; 
                });

            await Context.Channel.ModifyMessageAsync(test.ActionsMsgID,
                x =>
                {
                    x.Content = $"{test.AdvMsg}{Environment.NewLine}Vos actions /adv [] : {string.Join(" - ", test.PossibleActions)}";
                });
        }
    }
}
