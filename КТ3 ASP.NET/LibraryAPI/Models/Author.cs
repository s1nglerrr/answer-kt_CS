namespace LibraryAPI.Models;

public class Author
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime BirthYear { get; set; }
}

public class AuthorDTO
{
    public string Name { get; set; } = string.Empty;
    public DateTime BirthYear { get; set; }
}