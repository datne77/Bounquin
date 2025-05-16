using BookReadingApp.Models;

public class BookDropdownGroupViewModel
{
    public string CategoryName { get; set; } = "";
    public List<Book> Books { get; set; } = new();
}
