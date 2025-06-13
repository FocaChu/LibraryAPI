using AutoMapper;
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
        private readonly Db _context;

        private readonly IMapper _mapper;

        public BookController(Db context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Book>>> GetAllBooks()
        {
            var books = await _context.Books
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
            var book = await _context.Books
                .Include(n => n.Genres)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }

            return Ok(book);
        }

        [HttpGet("author_nationality/{nationality}")]
        public async Task<ActionResult<List<Book>>> GetBooksByAuthorNationality(string nationality)
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genres)
                .Where(b => b.Author.Nationality == nationality)
                .ToListAsync();

            if (books == null || !books.Any())
            {
                return NotFound($"No books found for authors with nacionality {nationality}");
            }

            return Ok(books);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> UpdateBook(int id, [FromBody] BookDto book)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingBook = await _context.Books
                .Include(b => b.Genres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBook == null)
                return NotFound($"Book with ID {id} not found.");

            _mapper.Map(book, existingBook);

            var author = await _context.Authors.FindAsync(book.AuthorId);
            if (author == null)
                return NotFound($"Author with ID {book.AuthorId} not found.");

            existingBook.Author = author;

            existingBook.Genres.Clear();
            foreach (var genreId in book.GenreIds.Distinct())
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre == null)
                    return NotFound($"Genre with ID {genreId} not found.");

                existingBook.Genres.Add(genre);
            }

            await _context.SaveChangesAsync();
            return Ok(existingBook);
        }


        [HttpPost]
        public async Task<ActionResult<Book>> CreateBook([FromBody] BookDto newBook)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = await _context.Authors.FindAsync(newBook.AuthorId);

            if (author == null)
                return NotFound($"Author with ID {newBook.AuthorId} not found.");

            var book = _mapper.Map<Book>(newBook);
            book.Author = author;

            if (newBook.GenreIds == null || !newBook.GenreIds.Any())
                return BadRequest("At least one genre ID must be provided.");

            foreach (var genreId in newBook.GenreIds.Distinct())
            {
                var genre = await _context.Genres.FindAsync(genreId);
                if (genre == null)
                    return NotFound($"Genre with ID {genreId} not found.");

                book.Genres.Add(genre);
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookById), new { id = book.Id }, book);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound($"Book with ID {id} not found.");
            }
            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
