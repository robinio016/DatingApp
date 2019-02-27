using AutoMapper;
using System.Linq;
using DatingApp.API.Models;
using DatingApp.API.DTO;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(d=>d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                        opt.MapFrom(src => src.Photos.FirstOrDefault(p=>p.IsMain).Url);
                    })
                    .ForMember(dest => dest.Age, opt => {
                    opt.ResolveUsing(d=>d.DateOfBirth.CalculateAge());
                });
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageToReturnDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => {
                    opt.MapFrom(m =>m.Sender.Photos.FirstOrDefault(p =>p.IsMain).Url);
                })
                .ForMember(dest => dest.RecipientPhotoUrl, opt => {
                    opt.MapFrom(m =>m.Recipient.Photos.FirstOrDefault(p =>p.IsMain).Url);
                });
            

        }
    }
}