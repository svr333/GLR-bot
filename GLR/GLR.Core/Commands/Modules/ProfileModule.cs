using Discord;
using Discord.Commands;
using GLR.Core.Services;
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
        public async Task ShowProfile(string userName)
        {
            var id = await _profileService.GetProfileAsync(userName);

            var embed = new EmbedBuilder()
                .WithTitle($"User profile for {userName}")
                .WithUrl($"https://galaxylifereborn.com/profile/{userName}")
                .WithThumbnailUrl($"https://galaxylifereborn.com/uploads/avatars/{id}.png")
                .WithDescription($"\nThe user's id is {id}.")
                .Build();

            await ReplyAsync("", false, embed);
        }
    }
}
