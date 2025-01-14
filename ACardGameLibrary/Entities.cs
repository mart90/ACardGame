namespace ACardGameLibrary
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
        public double Rating { get; set; }
        public int KFactor { get; set; }

        public List<GamePlayer> GamesPlayed { get; set; }
        public List<Game> GamesWon { get; } = new();
    }

    public class Game
    {
        public int Id { get; set; }
        public string PlayedOnVersion { get; set; }
        public int ShuffleSeed { get; set; }

        public int? WinnerId { get; set; }
        public User? Winner { get; set; }

        public List<GamePlayer> Players { get; } = new();
    }

    public class GamePlayer
    {
        public int GameId { get; set; }
        public Game Game { get; set; }

        public int UserId { get; set; }
        public User Player { get; set; }

        public double? RatingChange { get; set; }
    }

    public class CardBuy
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public string CardName { get; set; }
        public int TurnNumber { get; set; }
    }

    public class LatestVersion
    {
        public string VersionString { get; set; }
    }
}
