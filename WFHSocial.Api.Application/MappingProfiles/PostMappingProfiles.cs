using AutoMapper;
using WFHSocial.Api.Domain.Posts.DTOs.PostModels;
using WHFSocial.Api.Domain.Posts.Models;

namespace WFHSocial.Api.Application.MappingProfiles
{
    public class PostMappingProfiles : Profile
    {
        public PostMappingProfiles()
        {
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.Id)))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.PostDateUtc, opt => opt.MapFrom(src => src.PostDateUtc))
                .ForMember(dest => dest.PostContent, opt => opt.MapFrom(src => src.PostContent))
                .ForMember(dest => dest.PostType, opt => opt.MapFrom(src => MapPostTypeToPostTypeDto(src.PostType)))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));
        }

        public static PostTypeDto MapPostTypeToPostTypeDto(PostType postTypeEnum)
        {
            return postTypeEnum switch
            {
                PostType.Personal => PostTypeDto.Personal,
                PostType.Group => PostTypeDto.Group,
                PostType.Recruitment => PostTypeDto.Recruitment,
                _ => PostTypeDto.Unknown,
            };
        }

        public static PostType MapPostTypeDtoToPostType(PostTypeDto postTypeEnum)
        {
            return postTypeEnum switch
            {
                PostTypeDto.Personal => PostType.Personal,
                PostTypeDto.Group => PostType.Group,
                PostTypeDto.Recruitment => PostType.Recruitment,
                _ => PostType.Unknown,
            };
        }
    }
}
