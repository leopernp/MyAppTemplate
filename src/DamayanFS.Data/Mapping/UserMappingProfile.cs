using AutoMapper;
using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Data.ContextModels;

namespace MyAppTemplate.Data.Mapping;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>()
            // AutoMapper handles the null-checks for you in these chains!
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.MapFrom(src => src.CreatedBy.FirstName))
            .ForMember(dest => dest.CreatedByLastName, opt => opt.MapFrom(src => src.CreatedBy.LastName))

            .ForMember(dest => dest.ModifiedByUsername, opt => opt.MapFrom(src => src.ModifiedBy.Username))
            .ForMember(dest => dest.ModifiedByFirstName, opt => opt.MapFrom(src => src.ModifiedBy.FirstName))
            .ForMember(dest => dest.ModifiedByLastName, opt => opt.MapFrom(src => src.ModifiedBy.LastName))

            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))

            // Security: Never map a password to a DTO
            .ForMember(dest => dest.Password, opt => opt.Ignore()) // Global protection
            .ReverseMap()
            // When going DTO -> Entity, we ignore Navigation Properties 
            // to prevent EF from trying to track/create new related entities accidentally
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UserActivityLogs, opt => opt.Ignore());
    }
}
