using System.ComponentModel.DataAnnotations;

public class ChangeUsernameViewModel
{
    [Required(ErrorMessage = "Das Feld bitte nicht leer lassen.")]
    [Display(Name = "Neuer Benutzername")]
    [MaxLength(50)]
    public string? NewUsername { get; set; }
    public string? UserId { get; set; }
}