using Tabele_ligowe.Models;

namespace Tabele_ligowe.ViewModels
{
    public class ScoreboardViewModel
    {
        public List<MatchViewModel> Matches { get; set; }
        public List<TeamViewModel> Teams { get; set; }
        public Guid SelectedLeagueId { get; set; }
        public Guid SelectedSeasonId { get; set; }
        public List<League> Leagues { get; set; }
        public List<Season> Seasons { get; set; }
        public int NumOfRounds { get; set; }

        public ScoreboardViewModel()
        {
            Matches = new List<MatchViewModel>();
            Teams = new List<TeamViewModel>();
            Leagues = new List<League>();
            Seasons = new List<Season>();
        }
    }
}
