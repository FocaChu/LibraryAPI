using System.ComponentModel.DataAnnotations;

namespace LibraryAPI
{
    public class GenreDto
    {
        [Required]
        public string Name { get; set; }
    }
}
