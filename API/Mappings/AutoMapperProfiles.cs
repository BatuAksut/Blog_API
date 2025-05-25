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
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName));

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.ApplicationUserId))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.ApplicationUser.UserName))
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => $"{src.ApplicationUser.Firstname} {src.ApplicationUser.Lastname}"));

            CreateMap<BlogPost, BlogPostDto>()
    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.ApplicationUser));
            CreateMap<CreateBlogPostDto, BlogPost>().ReverseMap();
            CreateMap<UpdateBlogPostDto, BlogPost>().ReverseMap();

        }

    }
}
