﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using TheBlacksmith.Services;

namespace TheBlacksmith
{
    class Program
    {
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private IConfiguration _config;

        public async Task MainAsync()
        {

            _client = new DiscordSocketClient();
            _client.Log += Log;

            _config = BuildConfig();
            var services = ConfigureServices();
            await services.GetRequiredService<CommandHandlingService>().InitializeAsync(services);
            await services.GetRequiredService<ReactionHandlingService>().InitializeAsync(services);
            await services.GetRequiredService<ButtonHandlingService>().InitializeAsync(services);
            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            //_client.Disconnected => peut être un moyen de reco le bot

            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<ButtonHandlingService>()
                // Logging
                //.AddLogging()
                //.AddSingleton<LogService>()
                // Extra
                .AddSingleton(_config)
                // Add additional services here...
                .AddSingleton<ReactionHandlingService>()
                .BuildServiceProvider();
        }

        private IConfiguration BuildConfig()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
