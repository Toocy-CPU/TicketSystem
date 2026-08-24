using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace TicketSystem.Models
{
    public class Project
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeadLine { get; set; }
        public bool ProjectClosed { get; set; } = false;

        // tickets zum project
        public ICollection<Ticket>? Tickets { get; set; }

        // viewmodel prop
        [NotMapped]
        public string? DescriptionPreview => string.IsNullOrEmpty(Description) ? null : Description.Length > 40 ? Description.Substring(0, 40) + "..." : Description;
        [NotMapped]
        public int OpenTickets { get; set; }
        [NotMapped]
        public int ClosedTickets { get; set; }
    }
}
