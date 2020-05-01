using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GLR.Core.Entities;
using GLR.Core.Services.DataStorage;

namespace GLR.Core.Services
{
    public class ProfileService
    {
        private HttpClient _webClient;

        public ProfileService()
        {
            _webClient = new HttpClient();
        }
        
        internal async Task<Profile> GetFullProfileAsync(string input)
        {
            Profile profile = new Profile();
            var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            profile.Id = await GetIdAsync(input);
            profile.Username = await GetUsernameAsync(profile.Id);
            profile.Url = $"https://galaxylifereborn.com/profile/{profile.Username.Replace(" ", "%20")}";
            profile.ImageUrl = $"https://galaxylifereborn.com/uploads/avatars/{profile.Id}.png?t={currentUnixTime}";

            profile.AmountOfFriends = await GetAmountOfFriendsAsync(profile.Id);
            profile.AmountOfIncomingRequests = await GetAmountOfIncomingRequestsAsync(profile.Id);
            profile.AmountOfOutgoingRequests = await GetAmountOfOutgoingRequestsAsync(profile.Id);

            profile.RankInfo = await GetRankInfoAsync(profile.Id);
            profile.CreationDate = await GetCreationDateAsync(profile.Id);
            
            return profile;
        }

        public async Task<ulong> GetIdAsync(string input)
        {
            // check if input is username
            var response = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={input}&t=i");
            var stringId = await response.Content.ReadAsStringAsync();
            
            // if returns "" => no such user exists
            if (!string.IsNullOrEmpty(stringId)) return ulong.Parse(stringId);

            // check if given input is id
            response = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={input}&t=n");
            var name = await response.Content.ReadAsStringAsync();

            // if it's empty, there is no user for the given input
            // throw so we catch it in the CommandExecuted event
            if (string.IsNullOrEmpty(name)) throw new Exception("Profile404");

            // now we know the input was an id
            return ulong.Parse(input);
        }

        private async Task<string> GetUsernameAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=n");
            var username = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            
            return username;
        }

        private async Task<int> GetAmountOfFriendsAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=f");
            var friendsAsString = await result.Content.ReadAsStringAsync();

            var friendIds = friendsAsString.Split(", ");
            if (friendIds[0] == "") friendIds = null;
            return friendIds is null ?  0 : friendIds.Length;
        }

        private async Task<int> GetAmountOfIncomingRequestsAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=r");
            var requestsAsString = await result.Content.ReadAsStringAsync();

            var userIds = requestsAsString.Split(", ");
            if (userIds[0] == "") userIds = null;
            return userIds is null ?  0 : userIds.Length;
        }

        private async Task<int> GetAmountOfOutgoingRequestsAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=s");
            var requestsAsString = await result.Content.ReadAsStringAsync();

            var userIds = requestsAsString.Split(", ");
            if (userIds[0] == "") userIds = null;
            return userIds is null ?  0 : userIds.Length;
        }

        private async Task<RankInfo> GetRankInfoAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=t");
            var stringRank =  await result.Content.ReadAsStringAsync();
            var isSuccess = Enum.TryParse(stringRank, out Rank rank);

            return new RankInfo()
            {
                Rank = rank
            };
        }

        private async Task<DateTime> GetCreationDateAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=c");
            var stringDate =  await result.Content.ReadAsStringAsync();

            return DateTime.Parse(stringDate);
        }

        public async Task<List<SimpleProfile>> GetFriendsAsync(ulong id)
        {
            var result = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=f_new");
            var friendsAsString = await result.Content.ReadAsStringAsync();

            var friendStringIds = friendsAsString.Split(", ");
            if (friendStringIds[0] == "") friendStringIds = null;

            if (friendStringIds is null) return null;


            var friends = new List<SimpleProfile>();
            
            foreach (var stringId in friendStringIds)
            {
                var data = stringId.Split('-');

                friends.Add(new SimpleProfile()
                {
                    Id = ulong.Parse(data[1]),
                    Username = data[0]
                });
            }

            return friends;
        }
    
        public async Task<Statistics> GetUserStatisticsAsync(ulong id)
        {
            var response = await _webClient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={id}&t=m");
            var statisticsCsv = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(statisticsCsv)) throw new Exception("Stats404");

            var values = statisticsCsv.Split(", ");
            Enum.TryParse(values[7], out AttackStatus attackStatus);
            Enum.TryParse(values[9], out Status status);

            return new Statistics()
            {
                Username = values[0],
                AllianceName = values[1],
                ExperiencePoints = ulong.Parse(values[2]),
                Starbase = ulong.Parse(values[3]),
                Colonies = ulong.Parse(values[4]),
                Level = ulong.Parse(values[5]),
                GalaxyName = values[6],
                AttackStatus = attackStatus,
                LastOnline = DateTime.Parse(values[8]),
                Status = status,
                MissionsCompleted = ulong.Parse(values[10])
            };
        }
    }
}
