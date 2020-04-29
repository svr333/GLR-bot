using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GLR.Core.Entities;

namespace GLR.Core.Services.Commands
{
    public class PaginatorService
    {
        private IEmote _next = new Emoji("▶️");
        private IEmote _previous = new Emoji("◀️");
        private IEmote _first = new Emoji("⏮️");
        private IEmote _last = new Emoji("️⏭️");
        private List<PaginatedMessage> _activeMessages;
        private DiscordSocketClient _client;

        public PaginatorService(DiscordSocketClient client)
        {
            _activeMessages = new List<PaginatedMessage>();
            _client = client;
            _client.ReactionAdded += OnReactionUpdated;
            _client.ReactionRemoved += OnReactionUpdated;
        }

        private async Task OnReactionUpdated(Cacheable<IUserMessage, ulong> msg, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var message = await msg.GetOrDownloadAsync();

            if (reaction.UserId == message.Author.Id) return;
            if (_activeMessages.Where(x => x.DiscordMessageId == message.Id).FirstOrDefault() is null) return;

            if (reaction.Emote.Name == _first.Name) await GoToFirstPageAsync(message.Id);
            else if (reaction.Emote.Name == _previous.Name) await GoToPreviousPageAsync(message.Id);
            else if (reaction.Emote.Name == _next.Name) await GoToNextPageAsync(message.Id);
            else if (reaction.Emote.Name == _last.Name) await GoToLastPageAsync(message.Id);
        }

        public async Task<IUserMessage> HandleNewPaginatedMessageAsync(SocketCommandContext context, IEnumerable<string> displayTexts, Embed embed)
        {
            var message = await context.Channel.SendMessageAsync("", false, embed);
            var test = new PaginatedMessage()
            {
                DiscordMessageId = message.Id,
                DiscordChannelId = message.Channel.Id,
                DisplayMessages = displayTexts.ToArray()
            };
            _activeMessages.Add(test);

            await AddPaginatorReactions(message);
            return message;
        }

        private async Task AddPaginatorReactions(IUserMessage message)
        {
            await message.AddReactionAsync(_first);
            await message.AddReactionAsync(_previous);
            await message.AddReactionAsync(_next);
            //await message.AddReactionAsync(_last);
        }

        private async Task HandleUpdateMessagePagesAsync(PaginatedMessage msg)
        {
            var message = await (_client.GetChannel(msg.DiscordChannelId) as SocketTextChannel).GetMessageAsync(msg.DiscordMessageId) as SocketUserMessage;
            var oldEmbed = message.Embeds.First();

            // get correct messages to display
            var displayMessages = msg.DisplayMessages.Skip((msg.CurrentPage - 1) * 10).Take(10);

            // update title to correct page
            var title = oldEmbed.Title.Split(" | ").First();
            var newTitle = title + $" | Page {msg.CurrentPage}";

            var newEmbed = new EmbedBuilder()
            {
                Title = newTitle,
                Description = string.Join("\n", displayMessages),
                Color = oldEmbed.Color,
                Url = oldEmbed.Url
            }
            //.WithAuthor(oldEmbed.Author.Value.Name, oldEmbed.Author.Value.IconUrl, oldEmbed.Author.Value.Url)
            //.WithFooter(oldEmbed.Footer.Value.Text, oldEmbed.Footer.Value.IconUrl)
            .Build();

            await message.ModifyAsync(x => x.Embed = newEmbed);
        }

        private async Task GoToLastPageAsync(ulong id)
        {
            var paginatorMessage = _activeMessages.Find(x => x.DiscordMessageId == id);
            paginatorMessage.CurrentPage = paginatorMessage.TotalPages;
            await HandleUpdateMessagePagesAsync(paginatorMessage);
        }

        private async Task GoToFirstPageAsync(ulong id)
        {
            var paginatorMessage = _activeMessages.Find(x => x.DiscordMessageId == id);
            paginatorMessage.CurrentPage = 1;
            await HandleUpdateMessagePagesAsync(paginatorMessage);
        }

        private async Task GoToNextPageAsync(ulong id)
        {
            var paginatorMessage = _activeMessages.First(x => x.DiscordMessageId == id);
            paginatorMessage.CurrentPage++;
            await HandleUpdateMessagePagesAsync(paginatorMessage);
        }

        private async Task GoToPreviousPageAsync(ulong id)
        {
            var paginatorMessage = _activeMessages.Find(x => x.DiscordMessageId == id);
            paginatorMessage.CurrentPage--;
            await HandleUpdateMessagePagesAsync(paginatorMessage);
        }
    }
}
