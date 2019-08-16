using System.Linq;
using AutoMapper;
using DatingApp.API.DTOs;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForListDtos>()
            .ForMember(dest => dest.PhotoUrl,opt => {opt.MapFrom(src => src.Photos.FirstOrDefault(predicate=>predicate.IsMain).Url);})
            .ForMember(dest => dest.Age,opt => {opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());});

            CreateMap<User,UserForDetailsDtos>()
            .ForMember(dest => dest.PhotoUrl,opt => {opt.MapFrom(src => src.Photos.FirstOrDefault(predicate=>predicate.IsMain).Url);})
            .ForMember(dest => dest.Age,opt => {opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());});
            CreateMap<Photo,PhotosForDetailsDtos>();
            CreateMap<UserForUpdateDtos,User>();
            CreateMap<Photo,PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto,Photo>();

        }
        
    }
}