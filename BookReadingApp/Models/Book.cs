using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReadingApp.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required] // ✅ Bắt buộc CategoryId nhưng không đánh dấu `[Required]` cho navigation property
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; } // ❌ Không đặt `[Required]` ở đây

        [Required] // ✅ Bắt buộc UserId nhưng không Required cho navigation property
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; } // ❌ Không đặt `[Required]` ở đây

        public string? CoverImageUrl { get; set; }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; set; }
        public ICollection<Chapter>? Chapters { get; set; } = new List<Chapter>();
        public int ViewCount { get; set; } = 0;

        public virtual ICollection<Review> Reviews { get; set; }
    }
}
