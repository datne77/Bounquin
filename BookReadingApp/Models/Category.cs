using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookReadingApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên thể loại không được để trống!")]
        [StringLength(100, ErrorMessage = "Tên thể loại không được quá 100 ký tự!")]
        public string Name { get; set; }

        // Một thể loại có nhiều sách
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
