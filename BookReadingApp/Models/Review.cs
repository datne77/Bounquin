using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReadingApp.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // ⭐ 1 đến 5 sao

        [StringLength(1000)]
        public string? Comment { get; set; } // Bình luận kèm theo (nếu có)

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Liên kết với người dùng
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Liên kết với sách
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
