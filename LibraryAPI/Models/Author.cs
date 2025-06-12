using System.Text.Json.Serialization;

namespace LibraryAPI
{
    public class Author
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Birth { get; set; }

        public string Nacionality { get; set; }

        [JsonIgnore] public ICollection<Book> Books { get; set; }
    }
}
