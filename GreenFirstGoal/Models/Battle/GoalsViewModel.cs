using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenFirstGoal.Models.Battle
{
    public class GoalsViewModel
    {
        [Key]
        public int GoalsGameID { get; set; }
        public string FirstGoal { get; set; }
        public int GoalsTotalGoals { get; set; }
        public DateTime? GameDate { get; set; }
    }
}
