using Microsoft.EntityFrameworkCore;
using Tabele_ligowe.Models;
using Tabele_ligowe.ViewModels;

namespace Tabele_ligowe.Services
{
    public class ScoreBoardService : IScoreBoardService
    {
        private readonly IRepositoryService<UserFavoriteTeam> _userFavoriteTeamRepository; 
        private readonly IRepositoryService<Team> _teamRepository;
        private readonly IRepositoryService<Match> _matchRepository;
        private readonly IRepositoryService<League> _leagueRepository;
        private readonly IRepositoryService<Season> _seasonRepository;
        public ScoreBoardService(IRepositoryService<UserFavoriteTeam> userFavoriteTeamRepository,
            IRepositoryService<Team> teamRepository,
            IRepositoryService<Match> matchRepository,
            IRepositoryService<League> leagueRepository,
            IRepositoryService<Season> seasonRepository )
        {
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _leagueRepository = leagueRepository;
            _seasonRepository = seasonRepository;
            _userFavoriteTeamRepository = userFavoriteTeamRepository;
        }

        public ScoreboardViewModel MapScoreboardViewModel(Guid selectedLeagueId, Guid selectedSeasonId, string username)
        {
            var leagues = _leagueRepository.GetAllRecords();

            if (selectedLeagueId.Equals(Guid.Empty))
            {
                selectedLeagueId = leagues.FirstOrDefault().Id;
            }

            var seasons = _seasonRepository.FindBy(x => x.LeagueId.Equals(selectedLeagueId));

            if (selectedSeasonId.Equals(Guid.Empty))
            {
                selectedSeasonId = seasons.FirstOrDefault().Id;
            }

            var selectedLeague = leagues.First(l => l.Id == selectedLeagueId);
            var selectedSeason = seasons.First(s => s.Id == selectedSeasonId);

            var teams = _teamRepository
                .FindBy(t => t.Seasons.Contains(selectedSeason))
                .Include(t => t.HomeMatches)
                .Include(t => t.AwayMatches);

            var matches = _matchRepository
                .FindBy(x => x.LeagueRound == 1 && x.SeasonId == selectedSeason.Id);

            var numOfRounds = _matchRepository
                .FindBy(x => x.SeasonId == selectedSeason.Id)
                .OrderByDescending(x => x.LeagueRound)
                .First()
                .LeagueRound;

            var model = new ScoreboardViewModel
            {
                SelectedLeagueId = selectedLeague.Id,
                SelectedSeasonId = selectedSeason.Id,
                Leagues = leagues.ToList(),
                Seasons = seasons.ToList(),
                NumOfRounds = numOfRounds
            };

            foreach (var team in teams)
            {
                var teamViewModel = MapTeamViewModel(team, selectedSeason, username);
                model.Teams.Add(teamViewModel);
            }

            foreach (var match in matches)
            {

                var matchViewModel = MapMatchViewModel(match);
                model.Matches.Add(matchViewModel);
            }

            model.Teams = model.Teams.OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalsDifference)
                .ThenByDescending(t => t.GoalsScored)
                .ToList();

            return model;
        }

        public TeamViewModel MapTeamViewModel(Team team, Season season, string username)
        {
            var userFavoriteTeam = _userFavoriteTeamRepository
                .FindBy(x => x.Username.Equals(username) && x.TeamId.Equals(team.Id)).FirstOrDefault();

            bool favorite = false;

            if (userFavoriteTeam != null)
                favorite = true;

            var result = new TeamViewModel{ Id = team.Id, Name = team.Name, Favorite = favorite };
            
            foreach (var match in team.HomeMatches)
            {
                if (!match.Season.Equals(season)) continue;

                result.GoalsScored += match.HomeTeamGoals;
                result.GoalsConceded += match.AwayTeamGoals;
                result.MatchesPlayed += 1;

                if (match.HomeTeamGoals > match.AwayTeamGoals) 
                {
                    result.Wins += 1;
                    result.Points += 3;
                }
                else if (match.HomeTeamGoals == match.AwayTeamGoals)
                {
                    result.Draws += 1;
                    result.Points += 1;
                }
                else
                {
                    result.Losses += 1;
                }
            }

            foreach (var match in team.AwayMatches)
            {
                if (!match.Season.Equals(season)) continue;

                result.GoalsScored += match.AwayTeamGoals;
                result.GoalsConceded += match.HomeTeamGoals;
                result.MatchesPlayed += 1;

                if (match.HomeTeamGoals < match.AwayTeamGoals) 
                {
                    result.Wins += 1;
                    result.Points += 3;
                }
                else if (match.HomeTeamGoals == match.AwayTeamGoals)
                {
                    result.Draws += 1;
                    result.Points += 1;
                }
                else
                {
                    result.Losses += 1;
                }
            }

            result.GoalsDifference = result.GoalsScored - result.GoalsConceded;

            return result;
        }

        public MatchViewModel MapMatchViewModel(Match match)
        {
            MatchViewModel result = new MatchViewModel();

            result.HomeTeamName = match.HomeTeam.Name;
            result.HomeTeamGoals = match.HomeTeamGoals;

            result.AwayTeamName = match.AwayTeam.Name;
            result.AwayTeamGoals = match.AwayTeamGoals;

            return result;
        }

        public IEnumerable<MatchViewModel> GetMatchesByRound(int round, Guid selectedSeasonId) 
        {
            var result = new List<MatchViewModel>();
            
            var matches = _matchRepository
                .FindBy(x => x.LeagueRound == round && x.SeasonId == selectedSeasonId)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam);

            foreach (var match in matches)
            {
                var matchViewModel = MapMatchViewModel(match);

                result.Add(matchViewModel);
            }

            return result;
        }

        public IEnumerable<MatchViewModel> GetMatchesByTeamName(string name, Guid selectedSeasonId)
        {
            var result = new List<MatchViewModel>();

            var matches = _matchRepository
                .FindBy(x => x.HomeTeam.Name.Equals(name) || x.AwayTeam.Name.Equals(name))
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam);

            foreach (var match in matches)
            {
                if (!match.SeasonId.Equals(selectedSeasonId)) continue;

                var matchViewModel = MapMatchViewModel(match);

                result.Add(matchViewModel);
            }

            return result;
        }

        public void ToggleTeamFavorite(Guid teamId, string username)
        {
            var userFavoriteTeam = _userFavoriteTeamRepository
                .FindBy(x => x.Username.Equals(username) && x.TeamId.Equals(teamId)).FirstOrDefault();

            if (userFavoriteTeam == null)
            {
                _userFavoriteTeamRepository.Add(new UserFavoriteTeam { Id = new Guid(), TeamId = teamId, Username = username });
            }
            else
            {
                _userFavoriteTeamRepository.Delete(userFavoriteTeam);
            }
        }
    }
}
