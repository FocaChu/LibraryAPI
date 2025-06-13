using AutoMapper;
using LibraryAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutorController : ControllerBase
    {
        private readonly Db _context;
        private readonly IMapper _mapper;

        public AutorController(Db context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Author>>> GetAllAuthors()
        {
            var authors = await _context.Authors.ToListAsync();

            if (authors == null || !authors.Any())
            {
                return NotFound("No authors found.");
            }

            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthorById(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound($"Author with ID {id} not found.");
            }

            return Ok(author);
        }

        [HttpPost("author")]
        public async Task<ActionResult<Author>> CreateAuthor([FromBody] AuthorDto newAuthor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var author = _mapper.Map<Author>(newAuthor);

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAuthorById), new { id = author.Id }, author);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);

            if (author == null)
            {
                return NotFound($"Author with ID {id} not found.");
            }

            _context.Authors.Remove(author);

            await _context.SaveChangesAsync();

            return Ok(author);
        }
    }
}
