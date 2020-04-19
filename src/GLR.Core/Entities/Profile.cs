using System;


namespace GLR.Core.Entities
{
    public class Profile
    {
        public ulong Id { get; set; }
        public string UserName { get; set; }
        public RankInfo RankInfo { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public int AmountOfFriends { get; set; }
        public int AmountOfIncomingRequests { get; set; }
        public int AmountOfOutgoingRequests { get; set; }
    }
}
