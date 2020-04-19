using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GLR.Core.Entities;
using Newtonsoft.Json;

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

            profile.Url = $"https://www.galaxylifereborn.com/profile/{profile.UserName.Replace(" ", "%20")}";
            profile.ImageUrl = $"https://galaxylifereborn.com/uploads/avatars/{profile.Id}.png?t={currentUnixTime}";

            var result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={profile.Id}&t=c");
            var stringDate =  await result.Content.ReadAsStringAsync();

            profile.CreationDate = DateTime.Parse(stringDate);
            profile.AmountOfFriends = await GetAmountOfFriends(profile.Id);
            profile.AmountOfIncomingRequests = await GetAmountOfIncomingRequests(profile.Id);
            profile.AmountOfOutgoingRequests = await GetAmountOfOutgoingRequests(profile.Id);

            result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={profile.Id}&t=t");
            var stringRank =  await result.Content.ReadAsStringAsync();
            var success = Enum.TryParse(stringRank, out Rank rank);

            profile.RankInfo = new RankInfo()
            {
                Rank = rank
            };

            return profile;
        }
        
        private async Task<int> GetAmountOfFriends(ulong id)
        {
            var webclient = new HttpClient();
            var result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=f");
            var friendsAsString = await result.Content.ReadAsStringAsync();

            var friendIds = friendsAsString.Split(", ");
            if (friendIds[0] == "") friendIds = null;
            return friendIds is null ?  0 : friendIds.Length;

            /*var friends = await GetFriends(id);

            if (friends is null) return 0;
            return friends.Count;*/
        }

        private async Task<int> GetAmountOfIncomingRequests(ulong id)
        {
            var webclient = new HttpClient();
            var result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=r");
            var requestsAsString = await result.Content.ReadAsStringAsync();

            var userIds = requestsAsString.Split(", ");
            if (userIds[0] == "") userIds = null;
            return userIds is null ?  0 : userIds.Length;
        }

        private async Task<int> GetAmountOfOutgoingRequests(ulong id)
        {
            var webclient = new HttpClient();
            var result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=s");
            var requestsAsString = await result.Content.ReadAsStringAsync();

            var userIds = requestsAsString.Split(", ");
            if (userIds[0] == "") userIds = null;
            return userIds is null ?  0 : userIds.Length;
        }

        public async Task<List<Profile>> GetFriends(ulong id)
        {
            var webclient = new HttpClient();
            var result = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=f");
            var friendsAsString = await result.Content.ReadAsStringAsync();

            var friendIds = friendsAsString.Split(", ");
            if (friendIds[0] == "") friendIds = null;

            if (friendIds is null) return null;

            var friends = new List<Profile>();

            for (int i = 0; i < friendIds.Length; i++)
            {
                var currentFriend = await GetProfileAsync(friendIds[i]);
                friends.Add(currentFriend);
            }

            return friends;
        }
    }
}
