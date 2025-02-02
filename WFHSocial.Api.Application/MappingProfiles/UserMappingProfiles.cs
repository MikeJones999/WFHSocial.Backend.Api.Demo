using AutoMapper;
using WFHSocial.Api.Domain.Authentication.Models;
using WFHSocial.Api.Domain.Users.DTOs.AuthModels;
using WFHSocial.Api.Domain.Users.DTOs.UserProfileModels;
using WFHSocial.Api.Domain.Users.Models;

namespace WFHSocial.Api.Application.MappingProfiles
{
    public class UserMappingProfiles: Profile
    {
        public UserMappingProfiles()
        {
            CreateMap<ApplicationUser, UserProfileResponse>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.SettingViews, opt => opt.MapFrom(src => src.UserSettings));

            CreateMap<UserSetting, UserSettingView>()
                 .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                 .ForMember(dest => dest.IsOn, opt => opt.MapFrom(src => src.IsOn))
                 .ForMember(dest => dest.SettingName, opt => opt.MapFrom(src => src.SettingName))
                 .ForMember(dest => dest.DateTimeModifiedUtc, opt => opt.MapFrom(src => src.DateTimeModifiedUtc));

            CreateMap<UserSettingView, UserSetting>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.IsOn, opt => opt.MapFrom(src => src.IsOn))
               .ForMember(dest => dest.SettingName, opt => opt.MapFrom(src => src.SettingName))
               .ForMember(dest => dest.DateTimeModifiedUtc, opt => opt.MapFrom(src => src.DateTimeModifiedUtc));
            //.ForMember(dest => dest.User, opt => opt.Ignore());

            CreateMap<UserRegister, ApplicationUser>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.DisplayName) ? src.UserName : src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.IsRecruiter, opt => opt.MapFrom(src => src.IsRecruiter));
        }
    }
}
