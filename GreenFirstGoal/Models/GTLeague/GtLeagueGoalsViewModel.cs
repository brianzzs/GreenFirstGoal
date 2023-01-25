using System.ComponentModel.DataAnnotations;

namespace GreenFirstGoal.Models.GTLeague
{
    public class GtLeagueGoalsViewModel
    {
        [Key]
        public int GoalsGameID { get; set; }
        public string FirstGoal { get; set; }
        public int GoalsTotalGoals { get; set; }
        public DateTime? GameDate { get; set; }
    }
}
