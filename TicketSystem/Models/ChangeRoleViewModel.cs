namespace TicketSystem.Models
{
    public class ChangeRoleViewModel
    {
        public string? UserId { get; set; }
        public string? NewRole { get; set; }
        public List<string>? Roles { get; set; }
    }
}
