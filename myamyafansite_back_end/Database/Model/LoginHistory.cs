using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class LoginHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }

    [Required]
    public DateTime LoginTime { get; set; } = DateTime.UtcNow;

    [MaxLength(45)] // Supports IPv4 and IPv6
    public string IpAddress { get; set; }

    [Required]
    public bool IsLogin { get; set; }  // true = login, false = logout

    [MaxLength(500)]
    public string DeviceInfo { get; set; }
}