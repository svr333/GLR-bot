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
                await message.Channel.SendMessageAsync($"I only respond in guilds.");
                return;
            }

            int argPos = 0;
            if (!message.HasPrefix(_client, out argPos, "!")) { return; }

            var context = new SocketCommandContext(_client, message);
            var result = await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> cmd, ICommandContext ctx, IResult result)
        {
            // TODO: Implement
            if (!cmd.IsSpecified) await ctx.Channel.SendMessageAsync(
                "Error occurred outside of commandcontext.\nPlease ask <@202095042372829184> for help unless it's obvious.");
            
            if (result.IsSuccess) 
            {
                await ctx.Message.AddReactionAsync(new Emoji("✅"));
                return;
            }

            var command = cmd.Value;

            switch (result.ErrorReason)
            {
                case "Profile404": await ctx.Channel.SendMessageAsync($"Galaxy Life Reborn profile for {ctx.Message.Content.Remove(0, 9)} not found.");
                break;

                default: await SendDefaultErrorMessage(ctx, cmd.Value, result.ErrorReason);
                break;
            }
        }

        private async Task SendDefaultErrorMessage(ICommandContext ctx, CommandInfo cmd, string error)
        {
            var embed = new EmbedBuilder()
            {
                Color = Color.Orange,
                Title = $"Error occurred while executing command '{cmd.Module.Name}: {cmd.Name}'",
                Description = $"{error}\nIf you think this error is out of place, please contact <@202095042372829184>.\n"
            }
            .Build();
        
            await ctx.Channel.SendMessageAsync("", false, embed);
        }
    }
}
