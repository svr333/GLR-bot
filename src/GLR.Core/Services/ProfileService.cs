using System;
using System.Net.Http;
using System.Threading.Tasks;
using GLR.Core.Entities;

namespace GLR.Core.Services
{
    public class ProfileService
    {
        internal async Task<Profile> GetProfileAsync(string userName) // sVr333
        {
            var webclient = new HttpClient();
            Profile profile = new Profile();
            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var response = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={userName}&t=i");

            var stringId = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(stringId))
            {
                var secResponse = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={userName}&t=n");
                var name = await secResponse.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(name)) return null;

                profile.UserName = name;
                profile.Id = ulong.Parse(userName);
            }
            else
            {
                profile.Id = ulong.Parse(stringId);

                var fourthResponse = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={profile.Id}&t=n");
                var actualUsername = await fourthResponse.Content.ReadAsStringAsync();

                profile.UserName = actualUsername;
            }

            profile.Url = $"https://www.galaxylifereborn.com/profile/{profile.UserName}";
            profile.ImageUrl = $"https://galaxylifereborn.com/uploads/avatars/{profile.Id}.png?t={currentUnixTime}";

            var result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={profile.Id}&t=c");
            var stringDate =  await result.Content.ReadAsStringAsync();
            profile.CreationDate = DateTime.Parse(stringDate);

            return profile;
        }
    }
}
