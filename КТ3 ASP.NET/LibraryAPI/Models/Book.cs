namespace LibraryAPI.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid GenreId { get; set; }
    public DateTime PublishedYear { get; set; }
}

public class BookDTO
{
    public string Title { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public Guid GenreId { get; set; }
    public DateTime PublishedYear { get; set; }
}