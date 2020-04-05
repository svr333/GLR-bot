using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GLR.Core.Services
{
    public class ProfileService
    {
        internal async Task<ulong> GetProfileAsync(string userName)
        {
            var webclient = new HttpClient();
            var response = await webclient.GetAsync($"https://galaxylifereborn.com/api/userinfo?u={userName}&t=i");

            var stringId = await response.Content.ReadAsStringAsync();

            return ulong.Parse(stringId);
        }
    }
}
