using ContradoTest.Models;
using ContradoTest.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ContradoTest.Controllers
{
    public class GitHubController : Controller
    {
        private readonly IGitHubService _gitHubService;

        public GitHubController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string username)
        {
            var result = await _gitHubService.Search(username);

            if (result != null)
            {
                return View("Results", result);
            }
            else
            {
                return View("Index", "User not found.");
            }
        }
    }
}

