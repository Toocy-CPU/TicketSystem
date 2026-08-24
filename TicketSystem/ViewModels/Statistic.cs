namespace TicketSystem.ViewModels
{
    public class Statistic
    {
        // Static field 
        public int OpenTickets { get; set; } = 0;
        public int ClosedTickets { get; set; } = 0;
        public int OpenProjects { get; set; } = 0;
        public int ClosedProjects { get; set; } = 0;
        public int Admins { get; set; } = 0;
        public int Devs { get; set; } = 0;
        public int Tester { get; set; } = 0;
    }
}
