using Microsoft.AspNetCore.Identity;

namespace TicketSystem.Models
{
    public class BlockTicket
    {
        public int Id { get; set; }

        public int TicketId { get; set; }               // das Ticket, das blockiert
        public Ticket? Ticket { get; set; }

        public int BlockedTicketId { get; set; }        // das blockierende Ticket
        public Ticket? BlockedTicket { get; set; }

        public DateTime BlocketAt { get; set; }
    }
}
