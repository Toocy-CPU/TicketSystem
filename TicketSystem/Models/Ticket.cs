using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class Ticket
    {

        public int Id { get; set; }
        [MaxLength(50)]
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        // zuweisung Sachbearbeiter
        public string? BearbeiterId { get; set; }
        public IdentityUser? Bearbeiter { get;set; }

        public DateTime HandeledAt { get; set; }

        // fremdschlüssel user id Ersteller
        public required string IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        // fremdschlüssel Project
        public required int ProjectId { get; set; }
        public Project? Project { get; set; }
        // geschlossen von wem und wann
        public string? CloserId { get; set; }
        public IdentityUser? Closer { get; set; }
        public DateTime ClosedAt { get; set; }
        public bool TicketClosed { get; set; } = false;

        // Liste der Hochgeladenen Dateien
        public ICollection<UploadFile>? UploadFiles { get; set; }

        // Kommentare
        public ICollection<Comment>? Comments { get; set; }

        // Tickets, die dieses Ticket blockiert
        public ICollection<BlockTicket>? BlockedTickets { get; set; } 

        //// Tickets, die dieses Ticket blockieren
        //public ICollection<BlockTicket> BlockingTickets { get; set; } = new List<BlockTicket>();

        // viewmodel prop
        [NotMapped]
        public string? DescriptionPreview => string.IsNullOrEmpty(Description) ? null : Description.Length > 100 ? Description.Substring(0, 100) + "..." : Description;

    }
}
