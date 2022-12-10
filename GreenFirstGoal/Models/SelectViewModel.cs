using Microsoft.AspNetCore.Mvc.Rendering;

namespace GreenFirstGoal.Models
{
    public class SelectViewModel
    {
        public string Player { get; set; }

        public string Player2 { get; set; }

        public List<SelectListItem> PlayersList { get; set; }
    }
}
