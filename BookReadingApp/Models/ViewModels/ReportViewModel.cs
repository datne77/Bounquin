namespace BookReadingApp.Models
{
    public class ReportViewModel
    {
        public int Id { get; set; }
        public string BookTitle { get; set; }
        public string ReporterName { get; set; }
        public string Reason { get; set; }
        public DateTime ReportedDate { get; set; }
        public int BookId { get; set; }
        public string? Status { get; set; }

    }
}
