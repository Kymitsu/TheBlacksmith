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
            if(p == null)
            {
                p = GameInstance.CreateNewPlayer(Context.Message.Author.Mention, name);
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} Character {p.Name} - LVL{p.Lvl} created");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} You already have a character({p.Name} - LVL{p.Lvl})");
            }
        }

        [Command("/adventure"), Alias("/adv")]
        public async Task NewAdventure(int advLvl)
        {
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
            //adventure next step (first step)
        }

        //[Command("/test")]
        //public async Task TestCommand()
        //{
        //    Player p = new Player(Context.User.Mention, "TestChar");
        //    Monster m = new Monster("Crab", 1);
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("```diff");
        //    //sb.Append(Environment.NewLine);
        //    sb.AppendLine($"You encounter {m.Name}: ({m.CurrentHp}/{m.Hp}hp)");
        //    sb.AppendLine($"You have {p.CurrentHp}/{p.Hp} left.");
        //    bool continueFight = true;
        //    int damageRoll;
        //    while (continueFight)
        //    {
        //        damageRoll = random.Next(10, 25);
        //        m.CurrentHp -= damageRoll;
        //        sb.AppendLine($"+ Dealt {damageRoll}hp to {m.Name}");
        //        if(m.CurrentHp <=0)
        //        {
        //            continueFight = false;
        //            sb.AppendLine("+ You win!");
        //            sb.AppendLine($"You have {p.CurrentHp}/{p.Hp} left.");
        //            continue;
        //        }

        //        damageRoll = random.Next(5, 20);
        //        p.CurrentHp -= damageRoll;
        //        sb.AppendLine($"- Lost {damageRoll}hp");
        //        if (p.CurrentHp <= 0)
        //        {
        //            continueFight = false;
        //            sb.AppendLine("- You lost!");
        //            continue;
        //        }
        //    }

        //    sb.Append("```");
        //    await Context.Channel.SendMessageAsync(sb.ToString());
        //}
    }
}
