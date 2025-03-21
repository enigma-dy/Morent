using System;
using AutoMapper;
using MoRent_V2.Models;
using MoRent_V2.Models.Dto;

namespace MoRent_V2.Models.Mapper;
public class CarMap : Profile
{
    public CarMap()
    {
        CreateMap<CarDto, Car>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DealerId, opt => opt.Ignore())
            .ForMember(dest => dest.Pictures, opt => opt.Ignore());
    }
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<Car, CarDto>()
            .ForMember(dest => dest.Pictures, opt => opt.MapFrom(src => src.Pictures))
            .ForMember(dest => dest.Dealer, opt => opt.MapFrom(src => src.Dealer == null ? null : new DealerDto
            {
                FullName = src.Dealer.FullName
            }));
        }
    }
}
