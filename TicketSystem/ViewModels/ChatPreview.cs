using Microsoft.AspNetCore.Identity;
using TicketSystem.Models;

namespace TicketSystem.ViewModels
{
    public class ChatPreview
    {
        public IdentityUser? User {get;set;}

        public Mail? LastMail { get; set; }
    }
}
