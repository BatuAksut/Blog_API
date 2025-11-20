using AutoMapper;
using Entities;
using Entities.DTO;
using static System.Net.Mime.MediaTypeNames;

namespace API.Mappings
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<ApplicationUser, UserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName)).ReverseMap();
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.ApplicationUser));
            CreateMap<CreateCommentDto, Comment>().ReverseMap();
            CreateMap<UpdateCommentDto, Comment>().ReverseMap();



            CreateMap<BlogPost, BlogPostDto>()
    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.ApplicationUser)).ReverseMap();
            CreateMap<CreateBlogPostDto, BlogPost>().ReverseMap();
            CreateMap<UpdateBlogPostDto, BlogPost>().ReverseMap();

            CreateMap<RegisterDto, ApplicationUser>();

        }

    }
}
