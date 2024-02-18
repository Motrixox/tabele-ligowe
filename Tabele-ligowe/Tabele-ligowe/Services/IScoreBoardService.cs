using Tabele_ligowe.Models;
using Tabele_ligowe.ViewModels;

namespace Tabele_ligowe.Services
{
    public interface IScoreBoardService
    {
        ScoreboardViewModel MapScoreboardViewModel(Guid selectedLeagueId, Guid selectedSeasonId, string username);
        TeamViewModel MapTeamViewModel(Team team, Season season, string username);
        MatchViewModel MapMatchViewModel(Match match);
        IEnumerable<MatchViewModel> GetMatchesByRound(int round, Guid selectedSeasonId);
        IEnumerable<MatchViewModel> GetMatchesByTeamName(string name, Guid selectedSeasonId);
        void ToggleTeamFavorite(Guid teamId, string username);
    }
}
