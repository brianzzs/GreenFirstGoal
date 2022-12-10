using FirstGoalBets.Data;
using GreenFirstGoal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GreenFirstGoal.Controllers
{
    public class HomeController : Controller
    {
        private readonly FirstGoalBetsDBContext _context;

        public HomeController(FirstGoalBetsDBContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var matchInfo = _context.Match.FromSqlRaw("select * from firstgoal.match order by Date desc limit 20");
            return View(matchInfo);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}