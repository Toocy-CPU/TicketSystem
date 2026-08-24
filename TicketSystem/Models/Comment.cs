using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class Comment
    {
        //  Ein Kommentar besteht mindestens aus den Attributen Inhalt, TicketID, Ersteller und Er
        //stellzeitpunkt
        public int Id { get; set; }
        [MaxLength(50)]
        public required string Title { get; set; }
        public required string Content { get; set; }
        // ersteller
        public required string IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        // zugehöriges Ticket
        public required int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
