using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tabele_ligowe.Models;
using Tabele_ligowe.Services;
using Tabele_ligowe.ViewModels;

namespace Tabele_ligowe.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IRepositoryService<UserFavoriteTeam> _userFavoriteTeamRepository;
		private readonly IRepositoryService<Team> _teamRepository;

		public HomeController(ILogger<HomeController> logger,
            IRepositoryService<UserFavoriteTeam> userFavoriteTeamRepository,
            IRepositoryService<Team> teamRepository)
		{
			_logger = logger;
			_userFavoriteTeamRepository = userFavoriteTeamRepository;
            _teamRepository = teamRepository;

        }

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Favorites()
		{
            var username = User?.Identity?.Name;

			var model = new List<FavoriteTeamViewModel>();

            if (username != null)
			{
				var favoriteTeams = _userFavoriteTeamRepository.FindBy(x => x.Username.Equals(username)).ToList();

				foreach (var favTeam in favoriteTeams)
				{
					var team = _teamRepository.GetSingle(favTeam.TeamId);
					model.Add(new FavoriteTeamViewModel{ Name = team.Name });
				}
            }

            return View(model);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
