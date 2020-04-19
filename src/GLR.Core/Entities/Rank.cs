namespace GLR.Core.Entities
{
    public class RankInfo
    {
        public Rank Rank { get; set; }
        public uint ColourValue 
        { 
            get 
            {
                if (Rank == Rank.Developer) return 480472;
                if (Rank == Rank.Moderator) return 2605694;
                if (Rank == Rank.Tester) return 16729674;
                if (Rank == Rank.Donator) return 15710778;
                else return 16777215;
            }
        }
    }

    public enum Rank
    {
        Member = 0,
        Donator = 1,
        Tester = 2,
        Moderator = 3,
        Developer = 5
    }
}
