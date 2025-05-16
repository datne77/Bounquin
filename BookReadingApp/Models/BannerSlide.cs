using System;
using System.ComponentModel.DataAnnotations;

namespace BookReadingApp.Models
{
    public class BannerSlide
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Subtitle { get; set; }

        public string? ImageUrl { get; set; }

        public int? LinkToBookId { get; set; }

        public int SortOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Book? Book { get; set; }
    }
}