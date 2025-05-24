using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myamyafansite_back_end.Database.Model;

public class Video
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
        public int DurationInSeconds { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        public DateTime UploadedDate { get; set; }
        
        [Required]
        [MaxLength(500)]
        public string Url { get; set; } // Master playlist URL (e.g., "/api/hls/{folder}/master.m3u8")

        public string? Poster { get; set; } // Base64 string of image (data URI format preferred)
    
}