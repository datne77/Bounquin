using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookReadingApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } // Họ và tên

        [StringLength(255)]
        public string ProfilePictureUrl { get; set; } = "/uploads/default-avatar.png"; // Ảnh đại diện

        [StringLength(500)]
        public string Bio { get; set; } = ""; // Mô tả cá nhân

        // Liên kết với sách đã đăng
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
        // ✅ Liên kết với danh sách bình luận của người dùng
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();


    }
}
