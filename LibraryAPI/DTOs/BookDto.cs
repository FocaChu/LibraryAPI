using System.ComponentModel.DataAnnotations;

namespace LibraryAPI
{
    public class BookDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int AuthorId { get; set; }

        [Required]
        public List<int> GenreIds { get; set; }
    }
}
