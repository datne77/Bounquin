using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookReadingApp.Models
{
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [Required]
        public string ReporterId { get; set; }

        [ForeignKey("ReporterId")]
        public ApplicationUser Reporter { get; set; }

        [Required]
        [StringLength(1000)]
        public string Reason { get; set; }

        public DateTime ReportedDate { get; set; } = DateTime.Now;

        // ✅ Trạng thái xử lý báo cáo
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
    }
}
