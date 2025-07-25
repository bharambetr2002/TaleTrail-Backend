using AutoMapper;
using TaleTrail.API.DTOs;
using TaleTrail.API.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BookRequestDTO, Book>();

        CreateMap<Book, BookResponseDTO>()
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => src.Authors.Select(a => a.Name)))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher.Name));
    }
}