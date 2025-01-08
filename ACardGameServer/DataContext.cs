using Microsoft.EntityFrameworkCore;

namespace ACardGameServer
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<LatestVersion> LatestVersion { get; set; }
        public DbSet<GameMove> GameMoves { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ServerMain.Config.GetSection("connectionString").Value.ToString());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("user");
                e.HasKey(e => e.Id);
                e.Property(e => e.PasswordHash).HasColumnName("password_hash");
                e.Property(e => e.Rating).HasColumnName("rating").ValueGeneratedOnAdd();
                e.Property(e => e.KFactor).HasColumnName("kfactor").ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Game>(e =>
            {
                e.ToTable("game");
                e.HasKey(e => e.Id);
                e.Property(e => e.WinnerId).HasColumnName("winner_id");
                e.Property(e => e.PlayedOnVersion).HasColumnName("game_version");
                e.Property(e => e.ShuffleSeed).HasColumnName("shuffle_seed");
                e.HasOne(e => e.Winner).WithMany(e => e.GamesWon);
            });

            modelBuilder.Entity<GamePlayer>(e =>
            {
                e.ToTable("game_player");
                e.HasKey(e => new
                {
                    e.GameId,
                    e.UserId
                });
                e.Property(e => e.GameId).HasColumnName("game_id");
                e.Property(e => e.UserId).HasColumnName("user_id");
                e.Property(e => e.RatingChange).HasColumnName("rating_change");
                e.HasOne(e => e.Game).WithMany(e => e.Players).HasForeignKey(e => e.GameId);
                e.HasOne(e => e.Player).WithMany(e => e.GamesPlayed).HasForeignKey(e => e.UserId);
            });

            modelBuilder.Entity<GameMove>(e =>
            {
                e.ToTable("game_move");
                e.HasKey(e => e.Id);
                e.Property(e => e.GameId).HasColumnName("game_id");
                e.Property(e => e.UserId).HasColumnName("user_id");
                e.Property(e => e.Type).HasColumnName("move_type");
                e.Property(e => e.CardId).HasColumnName("card_id");
            });

            modelBuilder.Entity<GameMoveAcceptParams>(e =>
            {
                e.ToTable("game_move_accept_params");
                e.HasKey(e => new
                {
                    e.GameMoveId,
                    e.Order
                });
                e.Property(e => e.GameMoveId).HasColumnName("game_move_id");
                e.Property(e => e.TargetedCardIdsString).HasColumnName("targeted_card_ids");
                e.Property(e => e.OptionsPickedString).HasColumnName("options_picked");
                e.Property(e => e.ShopCostPicked).HasColumnName("shop_cost_picked");
                e.HasOne(e => e.GameMove).WithMany(e => e.AcceptParams).HasForeignKey(e => e.GameMoveId);
            });

            modelBuilder.Entity<CardBuy>(e =>
            {
                e.ToTable("card_buy");
                e.HasKey(e => e.Id);
                e.Property(e => e.GameId).HasColumnName("game_id");
                e.Property(e => e.UserId).HasColumnName("user_id");
                e.Property(e => e.CardName).HasColumnName("card_name");
                e.Property(e => e.TurnNumber).HasColumnName("turn_number");
            });

            modelBuilder.Entity<LatestVersion>(e =>
            {
                e.ToTable("latest_version");
                e.HasNoKey();
                e.Property(e => e.VersionString).HasColumnName("latest_version");
            });
        }
    }

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

        public int WinnerId { get; set; }
        public User? Winner { get; set; }

        public List<GamePlayer> Players { get; } = new();
    }

    public class GamePlayer
    {
        public int GameId { get; set; }
        public Game Game { get; set; }

        public int UserId { get; set; }
        public User Player { get; set; }

        public double RatingChange { get; set; }
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
