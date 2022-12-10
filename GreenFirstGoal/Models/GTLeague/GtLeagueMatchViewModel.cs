using System.ComponentModel.DataAnnotations;

namespace GreenFirstGoal.Models.GTLeague
{
    public class GtLeagueMatchViewModel
    {
        [Key]
        public int GameID { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Jogador Casa")]
        public string HomePlayerName { get; set; }
        public string HomeTeamName { get; set; }
        [Display(Name = "")]
        public int HomeScore { get; set; }
        [Display(Name = "Jogador Visitante")]
        public string AwayPlayerName { get; set; }
        public string AwayTeamName { get; set; }
        [Display(Name = "")]
        public int AwayScore { get; set; }
        public int TotalGoals { get; set; }
    }
}
