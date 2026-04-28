namespace LibraryAPI.Models;

public class Genre
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class GenreDTO
{
    public string Name { get; set; } = string.Empty;
}