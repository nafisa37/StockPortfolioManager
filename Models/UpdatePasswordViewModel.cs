using System.ComponentModel.DataAnnotations;

public class UpdatePasswordViewModel
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Password must be between 5 and 20 characters.")]
    public string NewPassword { get; set; }
}
