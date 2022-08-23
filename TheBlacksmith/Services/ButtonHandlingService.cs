using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TheBlacksmith.Game;

namespace TheBlacksmith.Services
{
    public class ButtonHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;

        public ButtonHandlingService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.ButtonExecuted += ButtonHandler;

        }
        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        public async Task ButtonHandler(SocketMessageComponent component)
        {
            //// We can now check for our custom id
            //switch (component.Data.CustomId)
            //{
            //    // Since we set our buttons custom id as 'custom-id', we can check for it like this:
            //    default:
            //        // Lets respond by sending a message saying they clicked the button
            //        await component.RespondAsync($"{component.User.Mention} has clicked the button!");
            //        break;
            //}

            Player player = GameInstance.Players.FindByMention(component.User.Mention);

            Adventure adventure = GameInstance.ContinueAdventure(player, component.Data.CustomId);
            var builder = new ComponentBuilder();
            foreach (var item in adventure.PossibleActions)
            {
                builder.WithButton(item, item);
            }

            if (!adventure.ReSendMsg)
            {
                await component.UpdateAsync(x =>
                {
                    x.Content = $"```diff{Environment.NewLine}{adventure.EncounterMsg}```";
                    x.Components = builder.Build();
                });
            }
            else
            {
                await component.UpdateAsync(x =>
                {
                    x.Components = null;
                });
                var encounterMsg = await component.Channel.SendMessageAsync($"```diff{Environment.NewLine}{adventure.EncounterMsg}```", components: builder.Build());
                adventure.EncouterMsgID = encounterMsg.Id;

                adventure.ReSendMsg = false;
            }
        }
    }
}
