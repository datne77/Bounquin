using System.ComponentModel.DataAnnotations;

namespace BookReadingApp.Models.ViewModels
{
    public class ReviewViewModel
    {
        [Required]
        [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1 đến 5 sao.")]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }

        public int BookId { get; set; }
    }
}