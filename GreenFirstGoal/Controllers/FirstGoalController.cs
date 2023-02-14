using FirstGoalBets.Data;
using GreenFirstGoal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GreenFirstGoal.Controllers
{
    public class FirstGoalController : Controller
    {
        private readonly FirstGoalBetsDBContext _context;

        public FirstGoalController(FirstGoalBetsDBContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FirstGoalSelectBattle()
        {
            var distinctPlayers = from match in _context.Match
                                  select match.HomePlayerName;

            var vmPlayers = new SelectViewModel();

            vmPlayers.PlayersList = new List<SelectListItem>();

            foreach (var player in distinctPlayers.ToList().Distinct().ToList().OrderBy(x => x).ToList())
            {
                vmPlayers.PlayersList.Add(new SelectListItem { Text = player, Value = player });
            }
            return View(vmPlayers);
        }

        [HttpPost]
        public IActionResult FirstGoalBattle(SelectViewModel select)
        {
            var selectedPlayers = new List<String>
            {
                select.Player,
                select.Player2
            };

            var gamesList = new List<FirstGoalViewModel>();
            var totalsList = new List<TotalGoalsViewModel>();
            var Historylist = new List<HistoryViewModel>();
            var resultList = GetTotalWins(selectedPlayers, 20);

            var last5 = GetLastGamesBattle(selectedPlayers, 5);
            var last5Totals = GetTotalGoalsBattle(selectedPlayers, 5);
            var last10 = GetLastGamesBattle(selectedPlayers, 10);
            var last10Totals = GetTotalGoalsBattle(selectedPlayers, 10);
            var last15 = GetLastGamesBattle(selectedPlayers, 15);
            var last15Totals = GetTotalGoalsBattle(selectedPlayers, 15);
            var last20 = GetLastGamesBattle(selectedPlayers, 20);
            var last20Totals = GetTotalGoalsBattle(selectedPlayers, 20);

            var history = GetBattleHistory(selectedPlayers, 20);

            Historylist.AddRange(history);
            gamesList.Add(last5); gamesList.Add(last10); gamesList.Add(last15); gamesList.Add(last20);
            totalsList.Add(last5Totals); totalsList.Add(last10Totals); totalsList.Add(last15Totals); totalsList.Add(last20Totals);



            var modelView = new FirstGoalBattleViewModel
            {
                FirstGoalViewModel = gamesList,
                TotalGoalsViewModel = totalsList,
                HistoryViewModel = Historylist,
                WinsViewModel = resultList
            };

            var modelList = new List<FirstGoalBattleViewModel>
            {
                modelView
            };
            return View(modelList);
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

        private FirstGoalViewModel GetLastGamesBattle(List<string> playersList, int history)
        {
            var query = @$"SELECT * FROM firstgoal.match m INNER JOIN firstgoal.goals g ON m.gameID = g.GoalsGameID WHERE m.HomePlayerName = '{playersList[0]}' AND m.AwayPlayerName = '{playersList[1]}' AND (CASE WHEN g.Firstgoal = 'N' THEN g.GoalsTotalGoals = 0  ELSE TRUE END) OR m.HomePlayerName = '{playersList[1]}' AND m.AwayPlayerName = '{playersList[0]}' AND (CASE WHEN g.Firstgoal = 'N' THEN g.GoalsTotalGoals = 0  ELSE TRUE END) ORDER BY m.Date desc LIMIT {history}";
            var firstGoalsFromDB = _context.Goals.FromSqlRaw(query).ToList();

            var goalsPlayer1 = firstGoalsFromDB.Where(e => e.FirstGoal == playersList[0]).ToList();
            var goalsPlayer2 = firstGoalsFromDB.Where(e => e.FirstGoal == playersList[1]).ToList();
            var noGoals = firstGoalsFromDB.Where(e => e.FirstGoal == "N" || e.GoalsTotalGoals == 0).ToList();

            var viewModel = new FirstGoalViewModel();
            {
                viewModel.NameFirstPlayer = playersList[0];
                viewModel.NameSecondPlayer = playersList[1];
                viewModel.FirstGoalAmountFirstPlayer = goalsPlayer1.Count();
                viewModel.FirstGoalAmountSecondPlayer = goalsPlayer2.Count();
                viewModel.NoGoals = noGoals.Count();
            }
            return viewModel;
        }

        private TotalGoalsViewModel GetTotalGoalsBattle(List<string> playersList, int history)
        {
            var query = @$"select * from firstgoal.match where HomePlayerName = '{playersList[0]}' AND AwayPlayerName = '{playersList[1]}' OR HomePlayerName = '{playersList[1]}' AND AwayPlayerName = '{playersList[0]}' ORDER BY Date DESC LIMIT {history}";
            var matchFromDB = _context.Match.FromSqlRaw(query).ToList();
            var totalGoalsPlayer1 = new List<int>();
            var totalGoalsPlayer2 = new List<int>();
            var matchTotalGoals = new List<int>();

            var goalsPlayer1Home = matchFromDB.Where(e => e.HomePlayerName.Equals(playersList[0], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.HomeScore);
            foreach (var goal in goalsPlayer1Home)
            {
                totalGoalsPlayer1.Add(goal);
            }
            var goalsPlayer1Away = matchFromDB.Where(e => e.AwayPlayerName.Equals(playersList[0], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.AwayScore);
            foreach (var goal in goalsPlayer1Away)
            {
                totalGoalsPlayer1.Add(goal);
            }

            var goalsPlayer2Home = matchFromDB.Where(e => e.HomePlayerName.Equals(playersList[1], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.HomeScore);
            foreach (var goal in goalsPlayer2Home)
            {
                totalGoalsPlayer2.Add(goal);
            }

            var goalsPlayer2Away = matchFromDB.Where(e => e.AwayPlayerName.Equals(playersList[1], StringComparison.OrdinalIgnoreCase)).ToList().Select(e => e.AwayScore);
            foreach (var goal in goalsPlayer2Away)
            {
                totalGoalsPlayer2.Add(goal);
            }

            var matchGoals = matchFromDB.Select(e => e.TotalGoals);
            foreach (var goals in matchGoals)
            {
                matchTotalGoals.Add(goals);
            }


            var noGoals = matchFromDB.Where(e => e.TotalGoals == 0).Count();
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

        private List<HistoryViewModel> GetBattleHistory(List<string> playersList, int history)
        {
            var query = @$"select * from firstgoal.match where HomePlayerName = '{playersList[0]}' AND AwayPlayerName = '{playersList[1]}' OR HomePlayerName = '{playersList[1]}' AND AwayPlayerName = '{playersList[0]}' ORDER BY Date DESC LIMIT {history}";
            var matchHistory = _context.Match.FromSqlRaw(query).ToList();

            var viewModelList = new List<HistoryViewModel>();

            foreach (var game in matchHistory)
            {
                var viewModel = new HistoryViewModel()
                {
                    GameDate = game.Date,
                    TeamA = game.HomeTeamName,
                    TeamB = game.AwayTeamName,
                    PlayerA = game.HomePlayerName,
                    PlayerB = game.AwayPlayerName,
                    ScoreA = game.HomeScore,
                    ScoreB = game.AwayScore
                };
                viewModelList.Add(viewModel);
            }
            return viewModelList;
        }

        private List<WinsViewModel> GetTotalWins(List<string> playersList, int history)
        {
            var query = @$"select * from firstgoal.match where HomePlayerName = '{playersList[0]}' AND AwayPlayerName = '{playersList[1]}' OR HomePlayerName = '{playersList[1]}' AND AwayPlayerName = '{playersList[0]}' ORDER BY Date DESC LIMIT {history}";
            var player1WinsCount = 0;
            var player2WinsCount = 0;
            var drawCount = 0;
            var matchHistory = _context.Match.FromSqlRaw(query).ToList();

            var player1Home = matchHistory.Where(e => e.HomePlayerName == playersList[0]).ToList();
            var player1Away = matchHistory.Where(e => e.AwayPlayerName == playersList[0]).ToList();

            foreach (var game in player1Home)
            {
                if (game.HomeScore > game.AwayScore && game.HomeScore != game.AwayScore)
                {
                    player1WinsCount++;
                }
                else if (game.HomeScore < game.AwayScore && game.HomeScore != game.AwayScore)
                {
                    player2WinsCount++;
                }
                else
                {
                    drawCount++;
                }
            }

            foreach (var game in player1Away)
            {
                if (game.AwayScore > game.HomeScore && game.HomeScore != game.AwayScore)
                {
                    player1WinsCount++;
                }
                else if (game.AwayScore == game.HomeScore)
                {
                    drawCount++;
                }
                else
                {
                    player2WinsCount++;
                }
            }

            var listResults = new List<WinsViewModel>();
            var winsViewModel = new WinsViewModel()
            {
                AmountWinsPlayerA = player1WinsCount,
                AmountWinsPlayerB = player2WinsCount,
                AmountDraws = drawCount,
            };
            listResults.Add(winsViewModel);

            return listResults;
        }
    }
}