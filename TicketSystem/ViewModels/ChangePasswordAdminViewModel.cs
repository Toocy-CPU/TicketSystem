using System.ComponentModel.DataAnnotations;

namespace TicketSystem.ViewModels
{
    public class ChangePasswordAdminViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "Das Feld bitte nicht leer lassen.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Das Passwort muss mindestens 8 Zeichen lang sein.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
                            ErrorMessage = "Das Passwort muss mindestens 8 Zeichen lang sein und mindestens eine Zahl, einen Groß- und einen Kleinbuchstaben enthalten.")]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Passwort")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Das Feld bitte nicht leer lassen.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Die Passwörter stimmen nicht überein.")]
        [Display(Name = "Passwort bestätigen")]
        public string ConfirmPassword { get; set; }
    }
}
