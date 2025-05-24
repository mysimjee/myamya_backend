using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }  // Store hashed password only

    [Required]
    public bool AgreeToTerms { get; set; }

    public string? ProfilePicture { get; set; } = ""; // Base64-encoded image (e.g., "data:image/png;base64,...")

}