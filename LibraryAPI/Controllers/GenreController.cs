using AutoMapper;
using LibraryAPI.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly Db _context;
        private readonly IMapper _mapper;

        public GenreController(Db context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> GetAllGenres()
        {
            var genres = await _context.Genres.ToListAsync();

            // Check if any genres were found

            if (genres == null || !genres.Any())
            {
                return NotFound("No genres found.");
            }

            return Ok(genres);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetGenreById(int id)
        {
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound($"Genre with ID {id} not found.");
            }

            return Ok(genre);
        }

        [HttpPut("genre/{id}")]
        public async Task<ActionResult<Genre>> UpdateGenre(int id, [FromBody] GenreDto dto)
        {
            var existing = await _context.Genres.FindAsync(id);
            if (existing == null)
                return NotFound($"Genre with ID {id} not found.");

            _mapper.Map(dto, existing); // sobrescreve as propriedades do DTO no objeto existente

            await _context.SaveChangesAsync();
            return Ok(existing);
        }


        [HttpPost]
        public async Task<ActionResult<Genre>> CreateAuthor([FromBody] GenreDto newGenre)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = _mapper.Map<Genre>(newGenre);

            _context.Genres.Add(genre);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genre);
        }
    }
}
