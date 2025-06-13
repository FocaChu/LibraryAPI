using System.ComponentModel.DataAnnotations;

namespace LibraryAPI
{
    public class AuthorDto
    {
        [Required]
        public string Name { get; set; }

        [Range(1000, 2025)]
        public int Birth { get; set; }

        [Required]
        public string Nationality { get; set; }
    }
}
