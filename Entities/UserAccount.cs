using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), IsUnique = true)]  // unique index for Email
[Index(nameof(Username), IsUnique = true)]  // unique index for Username
public class UserAccount
{
    [Key] // Primary key
    public int UserId { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(20, ErrorMessage = "Max 20 characters allowed.")]
    [Column("Username")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [MaxLength(100, ErrorMessage = "Max 100 characters allowed.")]
    [Column("Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [MaxLength(20, ErrorMessage = "Max 20 characters allowed.")]
    [Column("Password")]
    public string Password { get; set; }

    public decimal Cash { get; set; } = 1000;

    public virtual ICollection<UserPortfolio> Portfolios { get; set; }
}
