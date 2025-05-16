using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReadingApp.Models
{
    public class ReadingHistory
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        public DateTime LastReadTime { get; set; } = DateTime.Now;

        // Navigation properties
        public ApplicationUser User { get; set; }
        public Book Book { get; set; }
    }
}
