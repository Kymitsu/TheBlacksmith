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

        [Command("/status")]
        public async Task GetPlayerStatus()
        {
            _ = Context.Message.DeleteAsync();
            Player player = GameInstance.Players.FindByMention(Context.Message.Author.Mention);
            Adventure adv = GameInstance.OnGoingAdventures.FirstOrDefault(x => x.Player == player);

            if (adv.ChannelId == Context.Channel.Id)
            {
                adv.ReSendMsg = true;
            }

            var msg = await Context.Channel.SendMessageAsync("", false, player.Status.GetCurrentPage().Build());

            player.Status.MessageIds.Add(msg.Id);
            //TODO: add msgId to ReactionHandling service
            await msg.AddReactionAsync(new Emoji("\U000025c0"));
            await msg.AddReactionAsync(new Emoji("\U000025b6"));
            await msg.AddReactionAsync(new Emoji("\U0001F504"));
        }

        [Command("/newAdventure"), Alias("/newAdv")]
        public async Task NewAdventure(int advLvl)
        {
            _ = Context.Message.DeleteAsync();
            Player player = GameInstance.Players.FindByMention(Context.Message.Author.Mention);
            Adventure adventure = GameInstance.OnGoingAdventures.FirstOrDefault(a => a.Player == player);
            if(adventure != null)
            {
                await Context.Channel.SendMessageAsync($"{player.Name} already has an ongoing adventure here: <#{adventure.ChannelId}>");
                
            }
            else
            {
                var channel = await Context.Guild.CreateTextChannelAsync(
                    $"{player.Name}-adventure",
                    (x) => {
                            x.CategoryId = GameInstance.ChannelCategory;
                            //TODO add permission to user
                        }
                    );
                adventure = GameInstance.CreateNewAdventure(channel.Id, advLvl, player);
                await channel.SendMessageAsync($"{player.Mention} Welcome {player.Name}! Good luck on your adventure.");


                var builder = new ComponentBuilder();
                foreach (var item in adventure.PossibleActions)
                {
                    builder.WithButton(item, item);
                }
                var encounterMsg = await channel.SendMessageAsync($"```diff{Environment.NewLine}{adventure.EncounterMsg}```", components:builder.Build());
                adventure.EncouterMsgID = encounterMsg.Id;
                
                //var actionMsg = await channel.SendMessageAsync(adventure.BuildAdventureMessage());
                //adventure.AdvMsgID = actionMsg.Id;
            }
            
        }

        [Command("/adventure"), Alias("/adv")]
        public async Task Adventure(string action)
        {
            _ = Context.Message.DeleteAsync();
            //TODO rajouter check channel id
            Player player = GameInstance.Players.FindByMention(Context.Message.Author.Mention);

            Adventure adventure = GameInstance.ContinueAdventure(player, action);

            if (!adventure.ReSendMsg)
            {
                await Context.Channel.ModifyMessageAsync(
                    adventure.EncouterMsgID,
                    x =>
                    {
                        x.Content = $"```diff{Environment.NewLine}{adventure.EncounterMsg}```";
                    });

                //await Context.Channel.ModifyMessageAsync(adventure.AdvMsgID,
                //    x =>
                //    {
                //        x.Content = adventure.BuildAdventureMessage();
                //    });
            }
            else
            {
                var encounterMsg = await Context.Channel.SendMessageAsync($"```diff{Environment.NewLine}{adventure.EncounterMsg}```");
                adventure.EncouterMsgID = encounterMsg.Id;

                //var actionMsg = await Context.Channel.SendMessageAsync(adventure.BuildAdventureMessage());
                //adventure.AdvMsgID = actionMsg.Id;

                adventure.ReSendMsg = false;
            }
        }

        
    }
}
