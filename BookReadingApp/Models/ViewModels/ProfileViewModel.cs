using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using BookReadingApp.Models;

namespace BookReadingApp.ViewModels
{
    public class ProfileViewModel
    {
        public string? Id { get; set; } // ✅ Dùng để xác định UserId khi cập nhật

        [StringLength(100)]
        public string FullName { get; set; }

        public string Email { get; set; } // Không chỉnh sửa

        [Phone]
        public string PhoneNumber { get; set; }

        public string ProfilePictureUrl { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public IFormFile ProfilePicture { get; set; } // Upload ảnh đại diện

        public List<Book> Books { get; set; } = new List<Book>();
    }
}
