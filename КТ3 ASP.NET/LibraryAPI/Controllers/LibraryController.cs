using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI.Controllers;

[ApiController]
[Route("library")]
public class LibraryController(LibraryContext context) : ControllerBase
{
    [HttpGet]
    [Route("authors")]
    public List<Author> GetAuthors()
    {
        return context.AuthorsContext.ToList();
    }

    [HttpPost]
    [Route("authors")]
    public void PostAuthors([FromBody] AuthorDTO dto)
    {
        context.AuthorsContext.Add(
            new Author()
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                BirthYear = dto.BirthYear
            }
        );
        context.SaveChanges();
    }

    [HttpPut]
    [Route("authors/{id}")]
    public void PutAuthors([FromRoute] Guid id, [FromBody] AuthorDTO dto)
    {
        Author entity = context.AuthorsContext.First(a => a.Id == id);
        entity.Name = dto.Name;
        entity.BirthYear = dto.BirthYear;
        context.SaveChanges();
    }

    [HttpDelete]
    [Route("authors/{id}")]
    public void DeleteAuthors([FromRoute] Guid id)
    {
        Author entity = context.AuthorsContext.First(a => a.Id == id);
        context.AuthorsContext.Remove(entity);
        context.SaveChanges();
    }

    [HttpGet]
    [Route("books")]
    public List<Book> GetBooks([FromQuery] string? author, [FromQuery] string? genre)
    {
        IQueryable<Book> query = context.BooksContext.AsQueryable();

        if (!string.IsNullOrEmpty(author))
        {
            query = query.Where(b => context.AuthorsContext
                .Any(a => a.Id == b.AuthorId && a.Name.Contains(author)));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            query = query.Where(b => context.GenresContext
                .Any(g => g.Id == b.GenreId && g.Name.Contains(genre)));
        }

        return query.ToList();
    }

    [HttpGet]
    [Route("books/{id}")]
    public ActionResult<Book> GetBookById([FromRoute] Guid id)
    {
        Book book = context.BooksContext.First(b => b.Id == id);
        return book;
    }

    [HttpPost]
    [Route("books")]
    public void PostBooks([FromBody] BookDTO dto)
    {
        context.BooksContext.Add(
            new Book()
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                AuthorId = dto.AuthorId,
                GenreId = dto.GenreId,
                PublishedYear = dto.PublishedYear
            }
        );
        context.SaveChanges();
    }

    [HttpPut]
    [Route("books/{id}")]
    public void PutBooks([FromRoute] Guid id, [FromBody] BookDTO dto)
    {
        Book entity = context.BooksContext.First(b => b.Id == id);
        entity.Title = dto.Title;
        entity.AuthorId = dto.AuthorId;
        entity.GenreId = dto.GenreId;
        entity.PublishedYear = dto.PublishedYear;
        context.SaveChanges();
    }

    [HttpDelete]
    [Route("books/{id}")]
    public void DeleteBooks([FromRoute] Guid id)
    {
        Book entity = context.BooksContext.First(b => b.Id == id);
        context.BooksContext.Remove(entity);
        context.SaveChanges();
    }

    [HttpGet]
    [Route("genres")]
    public List<Genre> GetGenres()
    {
        return context.GenresContext.ToList();
    }

    [HttpPost]
    [Route("genres")]
    public void PostGenres([FromBody] GenreDTO dto)
    {
        context.GenresContext.Add(
            new Genre()
            {
                Id = Guid.NewGuid(),
                Name = dto.Name
            }
        );
        context.SaveChanges();
    }

    [HttpPut]
    [Route("genres/{id}")]
    public void PutGenres([FromRoute] Guid id, [FromBody] GenreDTO dto)
    {
        Genre entity = context.GenresContext.First(g => g.Id == id);
        entity.Name = dto.Name;
        context.SaveChanges();
    }

    [HttpDelete]
    [Route("genres/{id}")]
    public void DeleteGenres([FromRoute] Guid id)
    {
        Genre entity = context.GenresContext.First(g => g.Id == id);
        context.GenresContext.Remove(entity);
        context.SaveChanges();
    }
}