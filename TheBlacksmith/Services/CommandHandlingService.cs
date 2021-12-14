using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TheBlacksmith.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            if (!message.Content.StartsWith("/")) return;
            //TODO : restriction sur un channel ou plutot un groupe de channel

            var context = new SocketCommandContext(_discord, message);

            //run command in another Task
            //should remove the "A MessageReceived handler is blocking the gateway task." error
            _ = Task.Run(async () => {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);

                if (result.Error.HasValue &&
                    result.Error.Value != CommandError.UnknownCommand)
                {
                    await Log($"{context.User.Username}  :  {rawMessage}  :  {result.ToString()}");
                    //await context.Channel.SendMessageAsync(context.User.Mention + " : " + result.ToString());
                }
            });
            
        }

        private Task Log(string msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
