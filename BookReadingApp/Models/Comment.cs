using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReadingApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // ✅ ID người dùng bình luận

        public ApplicationUser User { get; set; } // ✅ Navigation đến User

        [Required]
        public int ChapterId { get; set; } // ✅ Bình luận thuộc Chương nào

        [ForeignKey("ChapterId")]
        public Chapter Chapter { get; set; } // ✅ Navigation đến Chapter

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } // ✅ Nội dung bình luận (Giới hạn 1000 ký tự)

        public DateTime CreatedDate { get; set; } = DateTime.Now; // ✅ Thời gian bình luận
    }
}
