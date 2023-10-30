using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using P02_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {
            
        }
        public FootballBettingContext(DbContextOptions options)
            :base(options)
        {
            
        }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Town> Towns { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<PlayerStatistic> PlayersStatistics { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database = FootballBookmakerSystem; Integrated Security=true; Encrypt = false;");
            }
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>()
                .HasKey(x => new {x.GameId,x.PlayerId});

            modelBuilder.Entity<Team>(x =>
            {
                x.HasOne(x => x.PrimaryKitColor)
                .WithMany(x => x.PrimaryKitTeams)
                .HasForeignKey(x => x.PrimaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(x => x.SecondaryKitColor)
                .WithMany(x => x.SecondaryKitTeams)
                .HasForeignKey(x => x.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);
            });


            modelBuilder.Entity<Game>(x => 
                {
                    x.HasOne(g => g.HomeTeam)
                    .WithMany(t => t.HomeGames)
                    .HasForeignKey(t => t.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                x.HasOne(g => g.AwayTeam)
                .WithMany(t => t.AwayGames)
                .HasForeignKey(t => t.AwayTeamId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Player>()
                .HasOne(x => x.Town)
                .WithMany(x => x.Players)
                .HasForeignKey(x => x.TownId)
                .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Team>(x => 
            //    {
            //        x.HasMany(t => t.HomeGames)
            //        .WithOne(g => g.HomeTeam)
            //        .HasForeignKey(t => t.HomeTeamId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    x.HasMany(t => t.AwayGames)
            //    .WithOne(g => g.AwayTeam)
            //    .HasForeignKey(t => t.AwayTeamId)
            //    .OnDelete(DeleteBehavior.Restrict);
            //});

            //modelBuilder.Entity<PlayerStatistic>(x =>
            //    {
            //        x.HasOne(x => x.Player)
            //        .WithMany(x => x.PlayersStatistics)
            //        .HasForeignKey(x => x.PlayerId)
            //        .OnDelete(DeleteBehavior.Restrict);

            //    }
            //    );
            //modelBuilder.Entity<PlayerStatistic>(x =>
            //    {
            //        x.HasOne(x => x.Game)
            //        .WithMany(x => x.PlayersStatistics)
            //        .HasForeignKey(x => x.GameId)
            //        .OnDelete(DeleteBehavior.Restrict);
            //    }
            //    );
            base.OnModelCreating(modelBuilder);
        }
    }
}
