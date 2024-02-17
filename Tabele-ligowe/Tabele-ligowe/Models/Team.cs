namespace Tabele_ligowe.Models
{
    public class Team : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Match> HomeMatches { get; set; }
        public ICollection<Match> AwayMatches { get; set; }
        public ICollection<Season> Seasons { get; set; }
    }
}
