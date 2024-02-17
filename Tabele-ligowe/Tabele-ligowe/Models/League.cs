namespace Tabele_ligowe.Models
{
    public class League : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Season> Seasons { get; set; }
    }
}
