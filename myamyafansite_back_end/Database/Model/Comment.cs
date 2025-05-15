using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Optional target: either Video or Picture
    public Guid? VideoId { get; set; }
    public Guid? PictureId { get; set; }
}