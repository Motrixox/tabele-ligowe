using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tabele_ligowe.Models;
using Tabele_ligowe.Services;
using Tabele_ligowe.ViewModels;

namespace Tabele_ligowe.Controllers
{
    public class ScoreboardController : Controller
	{
		private readonly IScoreBoardService _scoreboardService;

		public ScoreboardController(IScoreBoardService scoreboardService)
		{
            _scoreboardService = scoreboardService;
        }

        public IActionResult Index(Guid selectedLeagueId, Guid selectedSeasonId)
        {
            var username = User?.Identity?.Name;

            var model = _scoreboardService.MapScoreboardViewModel(selectedLeagueId, selectedSeasonId, username);

            return View(model);
        }


        [HttpPost]
        public IActionResult UpdateMatchList(int round, Guid selectedSeasonId)
        {
			var result = _scoreboardService.GetMatchesByRound(round, selectedSeasonId);

            return Json(result);
        }

        [HttpPost]
        public IActionResult GetMatchesForTeam(string name, Guid selectedSeasonId)
        {
			var result = _scoreboardService.GetMatchesByTeamName(name, selectedSeasonId);

            return Json(result);
        }

        [HttpPost]
        public IActionResult ToggleTeamFavorite(Guid teamId)
        {
            var username = User?.Identity?.Name;

            if (username == null)
                return BadRequest("User not logged in");

            _scoreboardService.ToggleTeamFavorite(teamId, username);

            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
