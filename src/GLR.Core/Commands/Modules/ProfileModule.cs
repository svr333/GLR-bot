using Discord;
using Discord.Commands;
using GLR.Core.Services;
using System;
using System.Threading.Tasks;

namespace GLR.Core.Commands.Modules
{
    public class ProfileModule : GLRModule<SocketCommandContext>
    {
        private ProfileService _profileService;

        public ProfileModule(ProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("profile")]
        public async Task ShowProfile([Remainder]string userName)
        {
            var profile = await _profileService.GetProfileAsync(userName);
            if (profile == null) 
            {
                await ReplyAsync("Profile not found."); return;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"User profile for {profile.UserName}")
                .WithUrl(profile.Url)
                .WithThumbnailUrl(profile.ImageUrl)
                .WithDescription($"\nThe user's id is {profile.Id}.")
                .Build();

            await ReplyAsync("", false, embed);
        }

        [Command("profile")]
        public async Task ShowProfile()
        {
            var name = Context.User.Username;

            var profile = await _profileService.GetProfileAsync(name);
            if (profile == null) 
            {
                await ReplyAsync("Profile not found."); return;
            }

            var embed = new EmbedBuilder()
                .WithTitle($"User profile for {profile.UserName}")
                .WithUrl(profile.Url)
                .WithThumbnailUrl(profile.ImageUrl)
                .WithDescription($"\nThe user's id is {profile.Id}.")
                .Build();

            await ReplyAsync("", false, embed);
        }
    }
}
