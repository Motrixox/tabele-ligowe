using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tabele_ligowe.Models;
using Tabele_ligowe.Services;

namespace Tabele_ligowe.Controllers
{
	public class LeagueController : Controller
	{
		private readonly IRepositoryService<Team> _teamRepository;
		private readonly IRepositoryService<Match> _matchRepository;
		private readonly ScoreBoardService _scoreboardService;

		public LeagueController(IRepositoryService<Team> teamRepository, IRepositoryService<Match> matchRepository, ScoreBoardService scoreboardService)
		{
            _teamRepository = teamRepository;
            _matchRepository = matchRepository;
            _scoreboardService = scoreboardService;
        }

		public IActionResult Index()
		{
			var teams = _teamRepository
				.GetAllRecords()
				.Include(t => t.HomeMatches)
				.Include(t => t.AwayMatches);

			var matches = _matchRepository.FindBy(x => x.LeagueRound == 1);

			var model = new ScoreboardViewModel();

			foreach(var team in teams)
			{
				var teamViewModel = _scoreboardService.MapTeamViewModel(team);

				model.Teams.Add(teamViewModel);
			}

			foreach(var match in matches)
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
        public IActionResult UpdateMatchList(int id)
        {
			List<MatchViewModel> result = new List<MatchViewModel>();

            var matches = _matchRepository
				.FindBy(x => x.LeagueRound == id)
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
        public IActionResult GetMatchesForTeam(string name)
        {
			List<MatchViewModel> result = new List<MatchViewModel>();

            var matches = _matchRepository
				.FindBy(x => x.HomeTeam.Name.Equals(name) || x.AwayTeam.Name.Equals(name))
				.Include(m => m.HomeTeam)
				.Include(m => m.AwayTeam);

            foreach (var match in matches)
            {
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
