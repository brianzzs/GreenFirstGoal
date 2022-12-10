using Microsoft.EntityFrameworkCore;
using GreenFirstGoal.Models.GTLeague;
using GreenFirstGoal.Models.Battle;

namespace FirstGoalBets.Data
{
    public class FirstGoalBetsDBContext : DbContext
    {
        public FirstGoalBetsDBContext(DbContextOptions<FirstGoalBetsDBContext> options) : base(options)
        {

        }

        public DbSet<MatchViewModel> Match { get; set; }
        public DbSet<GoalsViewModel> Goals { get; set; }
        public DbSet<GtLeagueMatchViewModel> GtLeagueMatch { get; set; }
        public DbSet<GtLeagueGoalsViewModel> GtLeagueGoals { get; set; }
    }
}
