using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tabele_ligowe.Extensions;
using Tabele_ligowe.Models;

namespace Tabele_ligowe.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public DbSet<Team> Teams { get; set; }
		public DbSet<Match> Matches { get; set; }

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

            List<string> teams = new List<string> {
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

            foreach (var team in teams)
            {
                Teams.Add(new Team { Id = Guid.NewGuid(), Name = team });
            }

            SaveChanges();

            SeedLeagueRound(teams, 1);
            SeedLeagueRound(teams, 2);

            SaveChanges();
        }

        private void SeedLeagueRound(List<string> teams, int leagueRound)
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
                    LeagueRound = leagueRound
                });
            }
        }
    }
}
