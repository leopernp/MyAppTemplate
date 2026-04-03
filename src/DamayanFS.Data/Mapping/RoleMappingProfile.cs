using AutoMapper;
using DamayanFS.Contract.DTO;

namespace DamayanFS.Data.Mapping;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<ContextModels.Role, RoleDto>()
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.MapFrom(src => src.CreatedBy.FirstName))
            .ForMember(dest => dest.CreatedByLastName, opt => opt.MapFrom(src => src.CreatedBy.LastName))

            .ForMember(dest => dest.ModifiedByUsername, opt => opt.MapFrom(src => src.ModifiedBy.Username))
            .ForMember(dest => dest.ModifiedByFirstName, opt => opt.MapFrom(src => src.ModifiedBy.FirstName))
            .ForMember(dest => dest.ModifiedByLastName, opt => opt.MapFrom(src => src.ModifiedBy.LastName))

            .ReverseMap()
            // When going DTO -> Entity, ignore navigation properties
            // to prevent EF from trying to track/create new related entities
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Users, opt => opt.Ignore());
    }
}
