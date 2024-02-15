namespace Tabele_ligowe.Models
{
    public class Match : IEntity
    {
        public Guid Id { get; set; }

        public Guid HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        public Guid AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        public int HomeTeamGoals { get; set; }
        public int AwayTeamGoals { get; set; }

        public int LeagueRound { get; set; }
    }
}
