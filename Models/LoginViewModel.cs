using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username or Email is required.")]
    [MaxLength(20, ErrorMessage = "Max 20 characters allowed.")]
    [DisplayName("Username/Email")]

    public string UsernameOrEmail {get; set;}

    [Required(ErrorMessage = "Password is required.")]
    // [StringLength(20, MinimumLength =5, ErrorMessage = "Password must be between 5 and 20 characters.")]
    [DataType(DataType.Password)]
    public string Password {get; set;}
}