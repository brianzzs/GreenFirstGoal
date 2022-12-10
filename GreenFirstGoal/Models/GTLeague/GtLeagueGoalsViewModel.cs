using System.ComponentModel.DataAnnotations;

namespace GreenFirstGoal.Models.GTLeague
{
    public class GtLeagueGoalsViewModel
    {
        [Key]
        public int GameID { get; set; }
        public string FirstGoal { get; set; }
        public int TotalGoals { get; set; }
        public DateTime? GameDate { get; set; }
    }
}
