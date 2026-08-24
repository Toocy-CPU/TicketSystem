using Microsoft.AspNetCore.Identity;
using TicketSystem.Models;

namespace TicketSystem.ViewModels
{
    public class MailViewModel
    {
        public List<Mail> SendMails { get; set; }
        public List<Mail> GotMails { get; set; }
        public List<IdentityUser> Users { get; set; }
    }
}
