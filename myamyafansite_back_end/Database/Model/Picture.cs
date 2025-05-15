using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class Picture
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Title { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [Required]
    public DateTime UploadedDate { get; set; }
    
    [Required]
    public DateTime ReleaseDate { get; set; }

    // Optional: If you store file paths or URLs
    public string ImagePath { get; set; }  // or ImageUrl
}