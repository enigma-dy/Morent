using AutoMapper;
using MoRent_V2.Models;

namespace MoRent_V2.Models.Mapper;

public class ProfileMapper : Profile
{
    public ProfileMapper()
    {

        CreateMap<Register, MoRentUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));


        CreateMap<UpdateProfileModel, MoRentUser>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.ProfilePics, opt => opt.Ignore());
    }
}