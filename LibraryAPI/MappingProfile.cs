using AutoMapper;

namespace LibraryAPI
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Book ↔ BookDto
            CreateMap<BookDto, Book>()
                .ForMember(dest => dest.Author, opt => opt.Ignore())
                .ForMember(dest => dest.Genres, opt => opt.Ignore());

            // Author ↔ AuthorDto
            CreateMap<AuthorDto, Author>();
            CreateMap<Author, AuthorDto>();

            // Genre ↔ GenreDto
            CreateMap<GenreDto, Genre>();
            CreateMap<Genre, GenreDto>();
        }
    }
}
