using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GreenFirstGoal.Models.Battle
{
    public class MatchViewModel
    {
        [Key]
        public int GameID { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Jogador Casa")]
        public string HomePlayerName { get; set; }
        [Display(Name = "Time Casa")]
        public string HomeTeamName { get; set; }
        [Display(Name = "Casa")]
        public int HomeScore { get; set; }
        [Display(Name = "Jogador Visitante")]
        public string AwayPlayerName { get; set; }
        [Display(Name = "Time Visitante")]

        public string AwayTeamName { get; set; }
        [Display(Name = "Visitante")]
        public int AwayScore { get; set; }
        public int TotalGoals { get; set; }
    }
}
