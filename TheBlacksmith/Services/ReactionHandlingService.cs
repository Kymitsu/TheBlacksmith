using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TheBlacksmith.Game;

namespace TheBlacksmith.Services
{
    public class ReactionHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private IServiceProvider _provider;

        private List<string> _pagedMsgEmote;

        public ReactionHandlingService(IServiceProvider provider, DiscordSocketClient discord)
        {
            _discord = discord;
            _provider = provider;

            _discord.ReactionAdded += OnReactionAdded;
            _discord.ReactionRemoved += OnReactionRemoved;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;

            _pagedMsgEmote = new List<string>();
            _pagedMsgEmote.Add("\U000025c0");
            _pagedMsgEmote.Add("\U000025b6");
            _pagedMsgEmote.Add("\U0001F504");

        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return;

            if (_pagedMsgEmote.Contains(reaction.Emote.Name))
            {
                _ = Task.Run(() => UpdateStatusMessage(reaction.Emote.Name, reaction.User.Value.Mention, cache));
            }
        }

        private async Task OnReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.User.Value.IsBot) return;

            if (_pagedMsgEmote.Contains(reaction.Emote.Name))
            {
                _ = Task.Run(() => UpdateStatusMessage(reaction.Emote.Name, reaction.User.Value.Mention, cache));
            }
        }

        private async Task UpdateStatusMessage(string emote, string mention, Cacheable<IUserMessage, ulong> cache)
        {
            Player p = GameInstance.Players.FindByMention(mention);

            Embed embed;
            switch (emote)
            {
                case "\U000025c0":
                    embed = p.Status.GetPreviousPage().Build();
                    break;
                case "\U000025b6":
                    embed = p.Status.GetNextPage().Build();
                    break;
                case "\U0001F504":
                    embed = p.Status.GetCurrentPage().Build();
                    break;
                default:
                    throw new Exception("UpdateStatusMessage: something went wrong!");
            }

            var msg = await cache.GetOrDownloadAsync();
            if (p.Status.MessageIds.Contains(msg.Id))
            {
                await msg.ModifyAsync(x => {
                    x.Content = "";
                    x.Embed = embed;
                });
            }
        }
    }
}
