namespace Tabele_ligowe.Models
{
    public class Season : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid LeagueId { get; set; }
        public League League { get; set; }

        public ICollection<Team> Teams { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
