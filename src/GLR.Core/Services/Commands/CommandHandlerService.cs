using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GLR.Core.Extensions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace GLR.Core.Services.Commands
{
    public class CommandHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public CommandHandlerService(DiscordSocketClient client, CommandService commands, IServiceProvider services)
        {
            _commands = commands;
            _client = client;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            _client.MessageReceived += OnMessageReceived;
            _commands.CommandExecuted += OnCommandExecuted;
        }

        private async Task OnMessageReceived(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message)) return;
            if (message.Author == _client.CurrentUser) { return; }

            if (message.Channel is IPrivateChannel)
            {
                await message.Channel.SendMessageAsync($"I only respond in guilds.\n\nPS: Maxim is gay");
                return;
            }

            int argPos = 0;
            if (!message.HasPrefix(_client, out argPos, "!")) { return; }

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> cmd, ICommandContext context, IResult result)
        {
            if (result.Error == CommandError.UnknownCommand)
            {
                var embed = new EmbedBuilder().WithColor(Color.DarkPurple)
                .WithTitle("Got em!").WithUrl("https://www.youtube.com/watch?v=dQw4w9WgXcQ").Build();

                await context.Channel.SendMessageAsync("", false, embed);
            }    
        }
    }
}
