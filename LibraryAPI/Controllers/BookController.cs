using LibraryAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public Db _contex;

        public BookController(Db context)
        {
            _contex = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAllBooks()
        {
            var books = await _contex.Books
                .Include(n => n.Genres)
                .Include(b => b.Author)
                .ToListAsync();

            // Check if any employees were found
            if (books == null || !books.Any())
            {
                return NotFound("No books found.");
            }

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBookById(int id)
        {
            var book = await _contex.Books
                .Include(n => n.Genres)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            return Ok(book);
        }

        [HttpGet("author_nacionality/{string}")]
        public async Task<ActionResult<List<Book>>> GetBooksByAuthorNationality(string nacionality)
        {
            var books = await _contex.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.Author.Nacionality == nacionality)
                .ToListAsync();

            if (books == null || !books.Any())
            {
                return NotFound($"No books found for authors with nacionality {nacionality}");
            }

            return Ok(books);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> UpdateBook(int id, [FromBody] BookDto book)
        {
            if (book == null)
            {
                return BadRequest("Book data is invalid.");
            }

            var existingBook = await _contex.Books.FindAsync(id);

            if (existingBook == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            existingBook.Title = book.Title;

            existingBook.Author = await _contex.Authors.FindAsync(book.AuthorId);

            if (existingBook.Author == null)
            {
                return NotFound($"Author with ID {book.AuthorId} not found.");
            }

            foreach(var id_genero in book.GenreIds)
            {
                var genre = await _contex.Genres.FindAsync(id_genero);

                if (genre != null)
                {
                    existingBook.Genres.Add(genre);
                }
                else
                {
                    return NotFound($"Genre with ID {id_genero} not found.");
                }
            }

            _contex.Books.Update(existingBook);

            await _contex.SaveChangesAsync();

            return Ok(existingBook);
        }

        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] BookDto newBook)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = await _contex.Authors.FindAsync(newBook.AuthorId);

            if (author == null)
                return NotFound($"Author with ID {newBook.AuthorId} not found.");

            var book = new Book
            {
                Title = newBook.Title,
                Author = author
            };

            if(newBook.GenreIds == null || !newBook.GenreIds.Any())
                return BadRequest("At least one genre ID must be provided.");

            foreach (var genreId in newBook.GenreIds)
            {
                var genre = await _contex.Genres.FindAsync(genreId);

                if (genre == null)
                    return NotFound($"Genre with ID {genreId} not found.");

                book.Genres.Add(genre);
            }

            _contex.Books.Add(book);
            await _contex.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var book = await _contex.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            _contex.Books.Remove(book);

            await _contex.SaveChangesAsync();

            return Ok(book);
        }
    }
}
