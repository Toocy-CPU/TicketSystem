namespace TicketSystem.ViewModels
{
    public class UserWithRoles
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; } 
        public List<string>? Roles { get; set; }
    }
}
