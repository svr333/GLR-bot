using Discord;
using Discord.Commands;
using GLR.Net;
using GLR.Net.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GLR.Core.Commands.Modules
{
    public class ProfileModule : GLRModule
    {
        private GLRClient _client;

        public ProfileModule(GLRClient client)
        {
            _client = client;
        }

        [Command("profile", RunMode = RunMode.Async)]
        public async Task ShowProfile([Remainder]string user = "")
        {
            if (string.IsNullOrEmpty(user)) user = Context.User.Username;

            var id = await _client.GetIdAsync(user);
            var profile = await _client.GetProfileAsync(id);

            var embed = new EmbedBuilder()
                .WithTitle($"Game profile for {profile.Username}")
                .WithUrl(profile.Url)
                .WithThumbnailUrl(profile.ImageUrl)
                .WithDescription($"\nThis user has ID **{profile.Id}**." +
                                $"\n**{profile.Username}** is a **{profile.RankInfo.Rank}**.")
                .AddField("Friends", $"The user has **{profile.AmountOfFriends}** friends." +
                    $"\nThe user has **{profile.AmountOfIncomingRequests}** pending incoming requests." +
                    $"\nThe user has **{profile.AmountOfOutgoingRequests}** pending outgoing requests.")
                .WithFooter($"Account created on {profile.CreationDate.ToLongDateString()}")
                .WithColor(profile.RankInfo.ColourValue)
                .Build();

            await ReplyAsync("", false, embed);
        }

        [Command("friends", RunMode = RunMode.Async)]
        public async Task Friends(string user = "")
        {
            if (string.IsNullOrEmpty(user)) user = Context.User.Username;

            var id = await _client.GetIdAsync(user);
            var profile = await _client.GetProfileAsync(id);
            var friends = await _client.GetFriendsAsync(id);
            if (friends is null) await ReplyAsync("User doesn't have any friends!");
            
            var displayTexts = friends.Select(x => $"**{x.Username}** ({x.Id})");

            var templateEmbed = new EmbedBuilder()
                                .WithTitle($"Friends for {profile.Username}")
                                .WithColor(Color.DarkBlue)
                                .WithAuthor("", "", "")
                                .WithFooter("Friends are ordered by day you added them.", "");
            await SendPaginatedMessageAsync(displayTexts, templateEmbed);
        }

        [Command("statistics")][Alias("stats")]
        public async Task Stats(string user = "")
        {
            if (string.IsNullOrEmpty(user)) user = Context.User.Username;
            var id = await _client.GetIdAsync(user);
            var profile = await _client.GetProfileAsync(id);

            var stats = await _client.GetStatisticsAsync(id);

            var statusEmote = stats.Status == Status.Online ? "<:online:705571366622986351>" : "<:offline:705571366614597722>";

            await ReplyAsync("", false, new EmbedBuilder()
            {
                Title = $"Statistics for {stats.Username} ({id})",
                Url = profile.Url,
                Color = Color.DarkMagenta,
                ThumbnailUrl = $"https://galaxylifereborn.com/uploads/avatars/{id}.png?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                Description = $"\u200b\n<:exp:705566339070296104> {stats.Level} \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b <:starbase:705566339078815755> {stats.Starbase}" +
                              $"\n<:colonies:705566339120758864> {stats.Colonies} \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b \u200b <:quest:705569878169485334> {stats.MissionsCompleted}" +
                              $"\n{statusEmote} {stats.Status} \u200b \u200b \u200b \u200b \u200b \u200b \u200b <:lastonline:705566339179216957> {stats.LastOnline.ToShortDateString()}\n\u200b"
            }
            .WithFooter($"Requested by {Context.User.Username} | {Context.User.Id}")
            .Build());
        }
    
        [Command("leaderboard")][Alias("lb")]
        public async Task DisplayLeaderboards(string leaderboardType = "levels")
        {
            IList<string> displayTexts = new string[] { "Error while retrieving data." };

            if (leaderboardType == "chips") displayTexts = (await _client.GetTopChipsPlayers()).ToArray();
            else if (leaderboardType == "levels") displayTexts = (await _client.GetTopLevelPlayers()).ToArray();
            else throw new Exception("Wrong leaderboard type. Either choose `levels´ or `chips´.");
            
            for (int i = 0; i < displayTexts.Count(); i++)
            {
                displayTexts[i] = $"**#{i + 1}** | {displayTexts[i].Split(' ').First()}\n";
            }

            var templateEmbed = new EmbedBuilder()
                                .WithTitle($"{leaderboardType.ToUpperInvariant()} Leaderboard")
                                .WithColor(Color.Purple)
                                .WithAuthor("", "", "")
                                .WithFooter($"Requested by {Context.User.Username} | {Context.User.Id}", "");

            await SendPaginatedMessageAsync(displayTexts, templateEmbed);
        }
    }
}
