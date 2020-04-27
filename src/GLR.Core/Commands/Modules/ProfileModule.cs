using Discord;
using Discord.Commands;
using GLR.Core.Services;
using System.Linq;
using System.Threading.Tasks;

namespace GLR.Core.Commands.Modules
{
    public class ProfileModule : GLRModule
    {
        private ProfileService _profileService;

        public ProfileModule(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("profile", RunMode = RunMode.Async)]
        public async Task ShowProfile([Remainder]string userName = "")
        {
            if (string.IsNullOrEmpty(userName)) userName = Context.User.Username;

            var profile = await _profileService.GetFullProfileAsync(userName);
            if (profile == null) 
            {
                await ReplyAsync($"There is no user named `{userName}`.");
                return;
            }

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

        [Command("friends")]
        public async Task Friends(string user = "")
        {
            if (string.IsNullOrEmpty(user)) user = Context.User.Username;

            var profile = await _profileService.GetFullProfileAsync(user);
            var cachedFriends = await _profileService.GetFriendsAsync(profile.Id);
            if (cachedFriends is null) await ReplyAsync("User doesn't have any friends!");
            
            var displayTexts = cachedFriends.Select(x => x is null ? "Profile not cached in local database." : $"{x.RankInfo.Rank}: **{x.Username}** ({x.Id})").ToList();
            if (displayTexts.Count() >= 10) await ReplyAsync("You have more than 10 friends, and I haven't implemented paginator yet, sorry.");
            else await ReplyAsync(string.Join("\n", displayTexts));
        }
    }
}
