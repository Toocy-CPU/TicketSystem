using Microsoft.AspNetCore.Mvc.Rendering;
using TicketSystem.Models;

namespace TicketSystem.ViewModels
{
    public class FilterViewModel
    {
        public IEnumerable<Ticket> Liste1 { get; set; }

        public string SelectedCategory { get; set; }
        public string BearbeiterId { get; set; }

        public List<SelectListItem> CategoryOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "-1", Text = "Alle Bearbeiter" },
            new SelectListItem { Value = "-2", Text = "Ohne Bearbeiter" }
        };
    }
}
