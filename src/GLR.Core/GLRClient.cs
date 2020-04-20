using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GLR.Core.Services;
using GLR.Core.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GLR.Core
{
    public class GLRClient
    {
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public GLRClient(CommandService commands = null, DiscordSocketClient client = null)
        {
            _client = client ?? new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000
            });

            _commands = commands ?? new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                LogLevel = LogSeverity.Debug,
                SeparatorChar = '|'
            });
        }

        public async Task InitializeAsync()
        {
            _services = ConfigureServices();

            _client.Ready += OnReadyAsync;

            var token = Environment.GetEnvironmentVariable("GLRToken");

            await Task.Delay(10).ContinueWith(t => _client.LoginAsync(TokenType.Bot, token));
            await _client.StartAsync();

            await _services.GetRequiredService<CommandHandlerService>().InitializeAsync();
            await Task.Delay(-1);
        }

        private async Task OnReadyAsync()
            => await _client.SetGameAsync("Serving Galaxy Life Reborn profiles since 1842.");

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<CommandHandlerService>()
                .AddSingleton<ProfileService>()
                .BuildServiceProvider();
        }
    }
}
