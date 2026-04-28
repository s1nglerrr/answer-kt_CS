using LibraryAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Data;

public class LibraryContext : DbContext
{
    public DbSet<Author> AuthorsContext { get; set; }
    public DbSet<Book> BooksContext { get; set; }
    public DbSet<Genre> GenresContext { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("");
    }
}