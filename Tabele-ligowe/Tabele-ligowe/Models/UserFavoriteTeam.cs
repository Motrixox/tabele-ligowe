namespace Tabele_ligowe.Models
{
    public class UserFavoriteTeam : IEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public Guid TeamId { get; set; }
    }
}
