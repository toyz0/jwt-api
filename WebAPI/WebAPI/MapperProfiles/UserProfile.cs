using AutoMapper;
using WebAPI.Dtos;
using WebAPI.Extensions;
using WebAPI.Models;

namespace WebAPI.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateDto, User>().ForMember(
                dest => dest.Password,
                opt => opt.MapFrom(src => src.Password.ToMD5Hash()));

            CreateMap<User, UserDto>();
            CreateMap<User, UserCreateDto>();
        }
    }
}
