﻿using Discord;
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

            var displayRank = profile.RankInfo.Rank == Rank.Banned ? "**BANNED**" : $"a {profile.RankInfo.Rank}";

            var embed = new EmbedBuilder()
                .WithTitle($"Game profile for {profile.Username}")
                .WithUrl(profile.Url)
                .WithThumbnailUrl(profile.ImageUrl)
                .WithDescription($"\nThis user has ID **{profile.Id}**." +
                                $"\n**{profile.Username}** is {displayRank}.")
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

            var displayAlliance = stats.AllianceName == "None" ? "User is not in any alliance." : $"User is part of **{stats.AllianceName}**.";

            await ReplyAsync("", false, new EmbedBuilder()
            {
                Title = $"Statistics for {stats.Username} ({id})",
                Url = profile.Url,
                Color = Color.DarkMagenta,
                ThumbnailUrl = $"https://galaxylifereborn.com/uploads/avatars/{id}.png?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
                Description = $"{displayAlliance}\nUser is level **{stats.Level}**.\n\u200b"
            }
            .AddField("Experience", FormatNumbers(stats.ExperiencePoints), true)
            .AddField("Starbase", stats.Starbase, true)
            .AddField("Colonies", stats.Colonies, true)
            .AddField("Missions", stats.MissionsCompleted, true)
            .AddField("Status", stats.Status, true)
            .AddField("Last seen", stats.LastOnline.ToShortDateString(), true)
            .WithFooter($"Requested by {Context.User.Username} | {Context.User.Id}")
            .Build());
        }
    
        [Command("leaderboard")][Alias("lb")]
        public async Task DisplayLeaderboards(string leaderboardType = "levels")
        {
            IList<string> displayTexts = new string[] { "Error while retrieving data." };

            if (leaderboardType == "chips") displayTexts = (await _client.GetTopChipsPlayers()).ToArray();
            else if (leaderboardType == "levels") displayTexts = (await _client.GetTopLevelPlayers()).ToArray();
            else throw new Exception("Wrong leaderboard type. Either choose `levels´ or `chips`.");
            
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
    
        [Command("compare")][Alias("c")]
        public async Task CompareStats(string baseUser, string secondUser)
        {
            if (baseUser.ToLower() == secondUser.ToLower()) throw new Exception("Please enter two different users to compare.");

            var baseUserId = await _client.GetIdAsync(baseUser);
            var baseUserStats = await _client.GetStatisticsAsync(baseUserId);

            var secondUserId = await _client.GetIdAsync(secondUser);
            var secondUserStats = await _client.GetStatisticsAsync(secondUserId);

            var expDifference = Math.Round((decimal)baseUserStats.ExperiencePoints / secondUserStats.ExperiencePoints, 2);

            await ReplyAsync("", false, new EmbedBuilder()
            {
                Title = $"Comparison between {baseUserStats.Username} & {secondUserStats.Username}",
                Description = $"{baseUserStats.Username} has **{expDifference}x** the experience of {secondUserStats.Username}\n" +
                              $"Difference of **{FormatNumbers(Math.Abs((decimal)baseUserStats.ExperiencePoints - secondUserStats.ExperiencePoints))}** experience.\n\n" + 
                              $"{baseUserStats.Username} has **{FormatNumbers(baseUserStats.ExperiencePoints)}** experience and is level **{baseUserStats.Level}**.\n" +
                              $"{secondUserStats.Username} has **{FormatNumbers(secondUserStats.ExperiencePoints)}** experience and is level **{secondUserStats.Level}**.",
                Color = expDifference > 1 ? Color.DarkGreen : Color.DarkOrange
            }
            .Build());
        }

        [Command("compare")][Alias("c")]
        public async Task CompareStats(string userToCompare)
            => await CompareStats(Context.User.Username, userToCompare);

        [Command("buildawall")]
        public async Task GetNecessaryExpForLevel(string baseUser, string userToCompare, [Remainder]string fact = "")
        {
            if (baseUser.ToLower() == userToCompare.ToLower()) throw new Exception("Please enter two different users to compare.");

            var baseUserId = await _client.GetIdAsync(baseUser);
            var baseUserStats = await _client.GetStatisticsAsync(baseUserId);

            var secondUserId = await _client.GetIdAsync(userToCompare);
            var secondUserStats = await _client.GetStatisticsAsync(secondUserId);

            var expToGain = (long)secondUserStats.ExperiencePoints - (long)baseUserStats.ExperiencePoints;
            if (expToGain < 0) throw new Exception($"{baseUserStats.Username} is already ahead of {secondUserStats.Username}");
            else if (expToGain == 0) throw new Exception($"{baseUserStats.Username} & {secondUserStats.Username} have equally as much exp.");
            
            await ReplyAsync("", false, new EmbedBuilder()
            {
                Title = $"Exp needed to beat {secondUserStats.Username}",
                Description = $"{baseUserStats.Username} has **{FormatNumbers(baseUserStats.ExperiencePoints)}** experience and is level **{baseUserStats.Level}**.\n" +
                              $"{secondUserStats.Username} has **{FormatNumbers(secondUserStats.ExperiencePoints)}** experience and is level **{secondUserStats.Level}**.\n\n" +
                              $"{GetVisualizationForProgressNecessary(expToGain, fact)}",
                Color = Color.DarkMagenta
            }
            .Build());
        }

        private string GetVisualizationForProgressNecessary(decimal expDifference, string fact = "")
        {
            var r = new Random();

            uint wallToLevel5Price = 248000;
            uint expPerWallToLevel5 = 641;

            uint sbToLevel9Price = 121000000;
            uint expPerSbToLevel9 = 313701;

            uint expPerFirebitAttack = 38990;

            uint expPerWarpGateLevel6Destroyed = 3541;

            if (fact == "") 
            {
                var random = r.Next(0, 3);

                if (random == 0) fact = "firebit";
                else if (random == 1) fact = "walls";
                else if (random == 2) fact = "sb";
                else if (random == 3) fact = "warp gates";
            }

            switch (fact)
            {
                case "firebit": return $"User needs to fully defeat firebit **{FormatNumbers(Math.Round(expDifference / expPerFirebitAttack, 0))}** more times to beat them.";
                case "walls": return $"User needs to upgrade **{FormatNumbers(Math.Round(expDifference / expPerWallToLevel5, 0))}** more walls from lvl 4 to 5 to beat them.\n" +
                                     $"This would cost **{FormatNumbers(Math.Round((expDifference / expPerWallToLevel5) * wallToLevel5Price, 0))}** gold";
                case "sb": return $"User needs to upgrade **{FormatNumbers(Math.Round(expDifference / expPerSbToLevel9, 0))}** more starbases from level 8 to 9 to beat them.\n" + 
                                  $"This would cost **{FormatNumbers(Math.Round((expDifference / expPerSbToLevel9) * sbToLevel9Price, 0))}** gold.";
                case "warp gates": return $"User would need to destroy **{FormatNumbers(Math.Round(expDifference / expPerWarpGateLevel6Destroyed))}** more warp gates lvl 6 to beat them.";
                
                default: return $"Please enter a valid value (`firebit`, `walls`, `sb` or `warp gates`).";
            }
        }

        private string FormatNumbers(decimal experiencePoints)
        {
            // 10mil< 
            if (experiencePoints > 10000000) return $"{Math.Round(experiencePoints / 1000000, 1)}M";

            // 1mil< 
            else if (experiencePoints > 1000000) return $"{Math.Round(experiencePoints / 1000000, 2)}M";

            // 100K<
            else if (experiencePoints > 10000) return $"{Math.Round(experiencePoints / 1000, 1)}K";

            // 10K<
            else if (experiencePoints > 10000) return $"{Math.Round(experiencePoints / 1000, 2)}K";

            else return experiencePoints.ToString();
        }
    }
}
