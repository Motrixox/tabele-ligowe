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
		private readonly ScoreBoardService _scoreboardService;

		public LeagueController(IRepositoryService<Team> teamRepository,  ScoreBoardService scoreboardService)
		{
            _teamRepository = teamRepository;
            _scoreboardService = scoreboardService;
        }

		public IActionResult Index()
		{
			var teams = _teamRepository
				.GetAllRecords()
				.Include(t => t.HomeMatches)
				.Include(t => t.AwayMatches)
				.ToList();

			var matches = new List<Match>();

			var model = new ScoreboardViewModel();

			foreach(var team in teams)
			{
				matches.AddRange(team.HomeMatches);

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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
