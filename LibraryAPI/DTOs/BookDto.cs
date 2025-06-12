namespace LibraryAPI
{
    public class BookDto
    {
        public string Title { get; set; }

        public int AuthorId { get; set; }

        public List<int> Gender_Ids { get; set; }
    }
}
