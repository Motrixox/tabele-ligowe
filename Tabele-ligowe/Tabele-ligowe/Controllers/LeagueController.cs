using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tabele_ligowe.Models;
using Tabele_ligowe.Services;
using Tabele_ligowe.ViewModels;

namespace Tabele_ligowe.Controllers
{
    public class LeagueController : Controller
	{
		private readonly IRepositoryService<Team> _teamRepository;
		private readonly IRepositoryService<Match> _matchRepository;
		private readonly IRepositoryService<League> _leagueRepository;
		private readonly IRepositoryService<Season> _seasonRepository;
		private readonly ScoreBoardService _scoreboardService;

		public LeagueController(IRepositoryService<Team> teamRepository, 
			IRepositoryService<Match> matchRepository, 
			IRepositoryService<League> leagueRepository, 
			IRepositoryService<Season> seasonRepository, 
			ScoreBoardService scoreboardService)
		{
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _leagueRepository = leagueRepository;
            _seasonRepository = seasonRepository;
            _scoreboardService = scoreboardService;
        }

        public IActionResult Index(Guid selectedLeagueId, Guid selectedSeasonId)
        {
            if (selectedLeagueId.Equals(Guid.Empty))
            {
                selectedLeagueId = _leagueRepository.GetAllRecords().FirstOrDefault().Id;
            }

            if (selectedSeasonId.Equals(Guid.Empty))
            {
                selectedSeasonId = _seasonRepository.FindBy(x => x.LeagueId.Equals(selectedLeagueId)).FirstOrDefault().Id;
            }

            var leagues = _leagueRepository.GetAllRecords();
            var seasons = _seasonRepository.FindBy(x => x.LeagueId.Equals(selectedLeagueId));

            var selectedLeague = leagues.FirstOrDefault(l => l.Id == selectedLeagueId);
            var selectedSeason = seasons.FirstOrDefault(s => s.Id == selectedSeasonId);

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
                var teamViewModel = _scoreboardService.MapTeamViewModel(team, selectedSeason);
                model.Teams.Add(teamViewModel);
            }

            foreach (var match in matches)
            {

                var matchViewModel = _scoreboardService.MapMatchViewModel(match);
                model.Matches.Add(matchViewModel);
            }

            model.Teams = model.Teams.OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalsDifference)
                .ThenByDescending(t => t.GoalsScored)
                .ToList();

            return View(model);
        }


        [HttpPost]
        public IActionResult UpdateMatchList(int round, Guid selectedSeasonId)
        {
			List<MatchViewModel> result = new List<MatchViewModel>();

            var matches = _matchRepository
				.FindBy(x => x.LeagueRound == round && x.SeasonId == selectedSeasonId)
				.Include(m => m.HomeTeam)
				.Include(m => m.AwayTeam);

            foreach (var match in matches)
            {
                var matchViewModel = _scoreboardService.MapMatchViewModel(match);

                result.Add(matchViewModel);
            }

            return Json(result);
        }

        [HttpPost]
        public IActionResult GetMatchesForTeam(string name, Guid selectedSeasonId)
        {
			List<MatchViewModel> result = new List<MatchViewModel>();

            var matches = _matchRepository
				.FindBy(x => x.HomeTeam.Name.Equals(name) || x.AwayTeam.Name.Equals(name))
				.Include(m => m.HomeTeam)
				.Include(m => m.AwayTeam);

            foreach (var match in matches)
            {
                if (!match.SeasonId.Equals(selectedSeasonId)) continue;

                var matchViewModel = _scoreboardService.MapMatchViewModel(match);

                result.Add(matchViewModel);
            }

            return Json(result);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
