using System.ComponentModel.DataAnnotations;

namespace TicketSystem.Models
{
    public class UserCreateModel
    {
        [Required(ErrorMessage = "Das Feld bitte nicht leer lassen.")]
        [Display(Name = "Neuer Benutzername")]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Das Feld bitte nicht leer lassen.")]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "Das Passwort muss mindestens 8 Zeichen lang sein.")]
        //[RegularExpression(@"^(?=.[a-z])(?=.[A-Z])(?=.*\d).{8,}$",
        //            ErrorMessage = "Das Passwort muss mindestens 8 Zeichen lang sein und mindestens eine Zahl, einen Groß- und einen Kleinbuchstaben enthalten.")]
        //[DataType(DataType.Password)]
        //[Display(Name = "Neues Passwort")]
        public required string Password { get; set; }
        public required string Role { get; set; }
    }
}