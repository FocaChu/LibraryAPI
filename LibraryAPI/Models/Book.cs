namespace LibraryAPI
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public Author Author { get; set; }

        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
