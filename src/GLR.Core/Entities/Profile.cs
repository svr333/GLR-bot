using System;

namespace GLR.Core.Entities
{
    public class Profile
    {
        public ulong Id { get; set; }
        public string UserName { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
