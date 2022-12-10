using FirstGoalBets.Data;
using GreenFirstGoal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GreenFirstGoal.Controllers
{
    public class FirstGoalGTLeagueController : Controller
    {
        private readonly FirstGoalBetsDBContext _context;

        public FirstGoalGTLeagueController(FirstGoalBetsDBContext context)
        {
            _context = context;
        }

        public IActionResult FirstGoalSelectGTLeague()
        {
            var distinctPlayers = from match in _context.GtLeagueMatch
                                  select match.HomePlayerName;

            var vmPlayers = new SelectViewModel();

            vmPlayers.PlayersList = new List<SelectListItem>();

            foreach (var player in distinctPlayers.ToList().Distinct().ToList().OrderBy(x => x).ToList())
            {
                vmPlayers.PlayersList.Add(new SelectListItem { Text = player, Value = player });
            }
            return View(vmPlayers);
        }

        public ActionResult GetData(string player)
        {
            var confronto = from match in _context.Match
                            where match.AwayPlayerName != player && match.HomePlayerName == player
                            select match.AwayPlayerName;



            var vmPlayers = new List<string>();
            foreach (var playerSelect in confronto.ToList().Distinct().ToList())
                vmPlayers.Add(playerSelect);

            return Json(new { vmPlayers });
        }

        [HttpPost]
        public IActionResult FirstGoalGTLeague(SelectViewModel select)
        {
            var selectedPlayers = new List<String>
            {
                select.Player,
                select.Player2
            };

            var gamesList = new List<FirstGoalViewModel>();
            var totalsList = new List<TotalGoalsViewModel>();

            var last5 = GetLastGamesGTLeague(selectedPlayers, 5);
            var last5Totals = GetTotalGoalsGTLeague(selectedPlayers, 5);
            var last10 = GetLastGamesGTLeague(selectedPlayers, 10);
            var last10Totals = GetTotalGoalsGTLeague(selectedPlayers, 10);
            var last15 = GetLastGamesGTLeague(selectedPlayers, 15);
            var last15Totals = GetTotalGoalsGTLeague(selectedPlayers, 15);
            var last20 = GetLastGamesGTLeague(selectedPlayers, 20);
            var last20Totals = GetTotalGoalsGTLeague(selectedPlayers, 20);

            gamesList.Add(last5); gamesList.Add(last10); gamesList.Add(last15); gamesList.Add(last20);
            totalsList.Add(last5Totals); totalsList.Add(last10Totals); totalsList.Add(last15Totals); totalsList.Add(last20Totals);

            var testeModel = new FirstGoalBattleViewModel
            {
                FirstGoalViewModel = gamesList,
                TotalGoalsViewModel = totalsList
            };

            var modelList = new List<FirstGoalBattleViewModel>();
            modelList.Add(testeModel);
            return View(modelList);
        }

        private TotalGoalsViewModel GetTotalGoalsGTLeague(List<string> playersList, int history)
        {
            var matchFromDB = from match in _context.GtLeagueMatch
                              join goals in _context.GtLeagueGoals on match.GameID equals goals.GameID
                              where match.HomePlayerName == playersList[0] && match.AwayPlayerName == playersList[1] || match.AwayPlayerName == playersList[0] && match.HomePlayerName == playersList[1]
                              select match;

            var totalGoalsPlayer1 = new List<int>();
            var totalGoalsPlayer2 = new List<int>();
            var matchTotalGoals = new List<int>();


            var lastGames = matchFromDB.OrderByDescending(match => match.Date).ToList().Take(history).ToList();

            var goalsPlayer1Home = lastGames.Where(e => e.HomePlayerName.Equals(playersList[0], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.HomeScore);
            foreach (var goal in goalsPlayer1Home)
            {
                totalGoalsPlayer1.Add(goal);
            }
            var goalsPlayer1Away = lastGames.Where(e => e.AwayPlayerName.Equals(playersList[0], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.AwayScore);
            foreach (var goal in goalsPlayer1Away)
            {
                totalGoalsPlayer1.Add(goal);
            }

            var goalsPlayer2Home = lastGames.Where(e => e.HomePlayerName.Equals(playersList[1], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.HomeScore);
            foreach (var goal in goalsPlayer2Home)
            {
                totalGoalsPlayer2.Add(goal);
            }

            var goalsPlayer2Away = lastGames.Where(e => e.AwayPlayerName.Equals(playersList[1], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.AwayScore);
            foreach (var goal in goalsPlayer2Away)
            {
                totalGoalsPlayer2.Add(goal);
            }

            var matchGoals = lastGames.Select(e => e.TotalGoals);
            foreach (var goals in matchGoals)
            {
                matchTotalGoals.Add(goals);
            }


            var noGoals = lastGames.Where(e => e.TotalGoals == 0).Count();

            var perc = (noGoals * 100) / history;

            try
            {
                var viewModel = new TotalGoalsViewModel()
                {
                    NameFirstPlayer = playersList[0],
                    NameSecondPlayer = playersList[1],
                    FirstPlayerGoalsAmount = Math.Round(totalGoalsPlayer1.Average(), 2),
                    SecondPlayerGoalsAmount = Math.Round(totalGoalsPlayer2.Average(), 2),
                    MatchTotalGoals = Math.Round(matchTotalGoals.Average(), 2),
                    NoGoals = perc
                };
                return viewModel;
            }
            catch
            {
                var viewModel = new TotalGoalsViewModel()
                {
                    NameFirstPlayer = playersList[0],
                    NameSecondPlayer = playersList[1],
                    FirstPlayerGoalsAmount = totalGoalsPlayer1.Sum(),
                    SecondPlayerGoalsAmount = totalGoalsPlayer1.Sum(),
                    MatchTotalGoals = matchTotalGoals.Sum(),
                    NoGoals = perc
                };
                return viewModel;
            }

        }

        private FirstGoalViewModel GetLastGamesGTLeague(List<string> playersList, int history)
        {
            var firstGoalsFromDB = from match in _context.GtLeagueMatch
                                   join goals in _context.GtLeagueGoals on match.GameID equals goals.GameID
                                   where
                                   goals.TotalGoals > 0 && goals.FirstGoal != "N"
                                   && match.HomePlayerName == playersList[0] && match.AwayPlayerName == playersList[1] || goals.TotalGoals > 0 && goals.FirstGoal != "N" && match.AwayPlayerName == playersList[0] && match.HomePlayerName == playersList[1]
                                   select goals;

            var matchFromDB = from match in _context.GtLeagueMatch
                              join goals in _context.GtLeagueGoals on match.GameID equals goals.GameID
                              where match.HomePlayerName == playersList[0] && match.AwayPlayerName == playersList[1] || match.AwayPlayerName == playersList[0] && match.HomePlayerName == playersList[1]
                              select match;

            var lastGames = firstGoalsFromDB.OrderByDescending(match => match.GameDate).ToList().Take(history).ToList();

            var goalsPlayer1 = lastGames.Where(e => e.FirstGoal == playersList[0]).ToList();
            var goalsPlayer2 = lastGames.Where(e => e.FirstGoal == playersList[1]).ToList();
            var noGoals = lastGames.Where(e => e.FirstGoal == "N" && e.TotalGoals > 0).ToList();
            var viewModel = new FirstGoalViewModel();
            {
                viewModel.NameFirstPlayer = playersList[0];
                viewModel.NameSecondPlayer = playersList[1];
                viewModel.FirstGoalAmountFirstPlayer = goalsPlayer1.Count();
                viewModel.FirstGoalAmountSecondPlayer = goalsPlayer2.Count();
                if (noGoals == null || noGoals.Count() == 0)
                    viewModel.NoGoals = 0;
                else
                    viewModel.NoGoals = noGoals.Count();
            }
            return viewModel;
        }

        //private Firstgoal


    }
}
