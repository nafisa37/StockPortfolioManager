using System.ComponentModel.DataAnnotations;

public class RegistrationViewModel
{
    // [Required(ErrorMessage = "First name is required.")]
    // [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]
    // public string FirstName {get; set;}

    // [Required(ErrorMessage = "Last name is required.")]
    // [MaxLength(50, ErrorMessage = "Max 50 characters allowed.")]

    // public string LastName {get; set;}

    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(20, ErrorMessage = "Max 20 characters allowed.")]

    public string Username {get; set;}

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100, ErrorMessage = "Max 100 characters allowed.")]
    //[EmailAddress(ErrorMessage = "Please enter valid email.")]
    [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid email.")]

    public string Email {get; set;}

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(20, MinimumLength =5, ErrorMessage = "Password must be between 5 and 20 characters.")]
    [DataType(DataType.Password)]

    public string Password {get; set;}

    [Compare("Password", ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]

    [Display(Name = "Confirm Password")]
    public string ConfirmPassword {get; set;}
}