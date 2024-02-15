namespace Tabele_ligowe.Models
{
    public class ScoreboardViewModel
    {
        public List<MatchViewModel> Matches { get; set; }
        public List<TeamViewModel> Teams { get; set; }

        public ScoreboardViewModel()
        {
            Matches = new List<MatchViewModel>();
            Teams = new List<TeamViewModel>();
        }
    }
}
