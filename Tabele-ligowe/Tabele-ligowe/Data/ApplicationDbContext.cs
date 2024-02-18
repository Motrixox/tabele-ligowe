using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tabele_ligowe.Extensions;
using Tabele_ligowe.Models;

namespace Tabele_ligowe.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<League> Leagues { get; set; }
		public DbSet<Season> Seasons { get; set; }
		public DbSet<Team> Teams { get; set; }
		public DbSet<Match> Matches { get; set; }
		public DbSet<UserFavoriteTeam> UserFavoriteTeam { get; set; }

		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
            SeedDatabase();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeTeam)
                .WithMany(t => t.HomeMatches)
                .HasForeignKey(m => m.HomeTeamId);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayTeam)
                .WithMany(t => t.AwayMatches)
                .HasForeignKey(m => m.AwayTeamId);
        }

        private void SeedDatabase()
		{
            if (Teams.Count() > 0) return;


            var league = new League { Id = Guid.NewGuid(), Name = "Liga Belgijska" };
            var league2 = new League { Id = Guid.NewGuid(), Name = "Testowa liga" };

            var season = new Season { Id = Guid.NewGuid(), Name = "2022/23", LeagueId = league.Id };
            var season2 = new Season { Id = Guid.NewGuid(), Name = "2023/24", LeagueId = league.Id };
            var season3 = new Season { Id = Guid.NewGuid(), Name = "2022/23", LeagueId = league2.Id };
            var season4 = new Season { Id = Guid.NewGuid(), Name = "2023/24", LeagueId = league2.Id };

            Leagues.AddRange(league, league2);
            Seasons.AddRange(season, season2, season3, season4);

            List<string> teamNames = new List<string> {
                "Genk",
                "Union Saint-Gilloise",
                "Antwerp",
                "Club Brugge",
                "Gent",
                "Standard Liege",
                "Westerio",
                "Cercle Brugge",
                "Charleroi",
                "OHL",
                "Anderlecht",
                "STVV",
                "Mechelen",
                "Kortrijk",
                "Eupen",
                "Oostende",
                "Zulte-Waregem",
                "Seraing United",
            };

            List<string> teamNames2 = new List<string> {
                "test1",
                "test2",
                "test3",
                "test4",
            };

            foreach (var teamName in teamNames)
            {
                var team = new Team { Id = Guid.NewGuid(), Name = teamName };
                team.Seasons = new List<Season> { season, season2 };
                Teams.Add(team);
            }

            foreach (var teamName in teamNames2)
            {
                var team = new Team { Id = Guid.NewGuid(), Name = teamName };
                team.Seasons = new List<Season> { season3, season4 };
                Teams.Add(team);
            }

            SaveChanges();

            SeedLeagueRound(teamNames, 1, season.Id);
            SeedLeagueRound(teamNames, 2, season.Id);

            SeedLeagueRound(teamNames, 1, season2.Id);
            SeedLeagueRound(teamNames, 2, season2.Id);

            SeedLeagueRound(teamNames2, 1, season3.Id);
            SeedLeagueRound(teamNames2, 2, season3.Id);
            SeedLeagueRound(teamNames2, 3, season3.Id);

            SeedLeagueRound(teamNames2, 1, season4.Id);
            SeedLeagueRound(teamNames2, 2, season4.Id);

            SaveChanges();
        }

        private void SeedLeagueRound(List<string> teams, int leagueRound, Guid seasonId)
        {
            var teamsTemp = new Queue<string>(teams.Shuffle());

            for (int i = 0; i < teams.Count / 2; i++)
            {
                Matches.Add(new Match
                {
                    Id = Guid.NewGuid(),
                    HomeTeamId = Teams.First(x => x.Name.Equals(teamsTemp.Dequeue())).Id,
                    AwayTeamId = Teams.First(x => x.Name.Equals(teamsTemp.Dequeue())).Id,
                    HomeTeamGoals = Random.Shared.Next(0, 7),
                    AwayTeamGoals = Random.Shared.Next(0, 7),
                    LeagueRound = leagueRound,
                    SeasonId = seasonId
                });
            }
        }
    }
}
