using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class ViewingHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }

    [MaxLength(45)] // Supports IPv4 and IPv6
    public string IpAddress { get; set; }

    public Guid? VideoId { get; set; }
    public Guid? PictureId { get; set; }

    [Required]
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool Watched { get; set; }  // true = watched; false = just opened/previewed
}