using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Tabele_ligowe.Models;
using Tabele_ligowe.Services;
using Tabele_ligowe.ViewModels;

namespace Tabele_ligowe.Controllers
{
	public class HomeController : Controller
	{
		private readonly IRepositoryService<UserFavoriteTeam> _userFavoriteTeamRepository;

		public HomeController(IRepositoryService<UserFavoriteTeam> userFavoriteTeamRepository)
		{
			_userFavoriteTeamRepository = userFavoriteTeamRepository;
        }

		public IActionResult Index()
		{
			return View();
		}

		[Authorize]
		public IActionResult Favorites()
		{
            var username = User?.Identity?.Name;

			var model = new List<FavoriteTeamViewModel>();

            if (username != null)
			{
				var favoriteTeams = _userFavoriteTeamRepository
					.FindBy(x => x.Username.Equals(username))
					.Include(x => x.Team)
					.ToList();

				foreach (var favTeam in favoriteTeams)
				{
					model.Add(new FavoriteTeamViewModel{ Name = favTeam.Team.Name });
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
