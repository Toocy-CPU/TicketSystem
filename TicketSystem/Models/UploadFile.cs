using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class UploadFile
    {
        public int Id { get; set; }
        public required string Filename { get; set; }
        public required long Filesize { get; set; }
        public required string Type { get; set; }
        // user der hochläd
        public required string IdentityUserId { get; set; }
        public IdentityUser? IdentityUser { get; set; }
        public DateTime UploadedAt { get; set; }
        // zuweisung Ticket
        public required int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        // viewmodel prop
        [NotMapped]
        public string? FileNamePrev => string.IsNullOrEmpty(Filename) ? null : Filename.Length > 40 ? Filename.Substring(0, 40) + ".." + Type : Filename;

    }
}
