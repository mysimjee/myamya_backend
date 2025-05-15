using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class Rating
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }

    [Required]
    [Range(1, 5)] // Assuming a 1 to 5 rating scale
    public int Score { get; set; }

    
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Optional: You can add references to Video or Picture if rating is for them
    public Guid? VideoId { get; set; }
    public Guid? PictureId { get; set; }
}