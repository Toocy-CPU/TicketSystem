using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class Mail
    {
        public int Id { get; set; }
        public required string SenderId { get; set; }
        public IdentityUser? Sender { get; set; }
        public DateTime SendDate { get; set; }
        public required string Text { get; set; }
        public required string? EmpfangerId { get; set; }
        public IdentityUser? Empfanger { get; set; }
        public bool AbsenderAnzeigen { get; set; } = true;
        public bool EmpfangerAnzeigen { get; set; } = true;

        [NotMapped]
        public string? TextPreview => string.IsNullOrEmpty(Text) ? null : Text.Length > 40 ? Text.Substring(0, 40) + "..." : Text;
    }
}
