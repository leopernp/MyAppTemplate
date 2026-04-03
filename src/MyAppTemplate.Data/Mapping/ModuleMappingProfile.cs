using AutoMapper;
using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Data.ContextModels;

namespace MyAppTemplate.Data.Mapping;

public class ModuleMappingProfile : Profile
{
    public ModuleMappingProfile()
    {
        // ModuleType ? ModuleTypeDto
        CreateMap<ModuleType, ModuleTypeDto>()
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.MapFrom(src => src.CreatedBy.FirstName))
            .ForMember(dest => dest.CreatedByLastName, opt => opt.MapFrom(src => src.CreatedBy.LastName))
            .ForMember(dest => dest.ModifiedByUsername, opt => opt.MapFrom(src => src.ModifiedBy.Username))
            .ForMember(dest => dest.ModifiedByFirstName, opt => opt.MapFrom(src => src.ModifiedBy.FirstName))
            .ForMember(dest => dest.ModifiedByLastName, opt => opt.MapFrom(src => src.ModifiedBy.LastName))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
            .ForMember(dest => dest.Modules, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Modules, opt => opt.Ignore());

        // Module ? ModuleDto
        CreateMap<Module, ModuleDto>()
            .ForMember(dest => dest.ModuleTypeName, opt => opt.MapFrom(src => src.ModuleType.Name))
            .ForMember(dest => dest.ParentModuleName, opt => opt.MapFrom(src => src.ParentModule.Name))
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.MapFrom(src => src.CreatedBy.FirstName))
            .ForMember(dest => dest.CreatedByLastName, opt => opt.MapFrom(src => src.CreatedBy.LastName))
            .ForMember(dest => dest.ModifiedByUsername, opt => opt.MapFrom(src => src.ModifiedBy.Username))
            .ForMember(dest => dest.ModifiedByFirstName, opt => opt.MapFrom(src => src.ModifiedBy.FirstName))
            .ForMember(dest => dest.ModifiedByLastName, opt => opt.MapFrom(src => src.ModifiedBy.LastName))
            .ForMember(dest => dest.ChildModules, opt => opt.Ignore())
            .ForMember(dest => dest.CanView, opt => opt.Ignore())
            .ForMember(dest => dest.CanCreate, opt => opt.Ignore())
            .ForMember(dest => dest.CanUpdate, opt => opt.Ignore())
            .ForMember(dest => dest.CanDelete, opt => opt.Ignore())
            .ForMember(dest => dest.IsAccessible, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.ModuleType, opt => opt.Ignore())
            .ForMember(dest => dest.ParentModule, opt => opt.Ignore())
            .ForMember(dest => dest.ChildModules, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedBy, opt => opt.Ignore())
            .ForMember(dest => dest.RolePermissions, opt => opt.Ignore())
            .ForMember(dest => dest.UserPermissions, opt => opt.Ignore());

        // RoleModulePermission ? RoleModulePermissionDto
        CreateMap<RoleModulePermission, RoleModulePermissionDto>()
            .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.Name))
            .ForMember(dest => dest.ModuleTypeName, opt => opt.MapFrom(src => src.Module.ModuleType.Name))
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.MapFrom(src => src.CreatedBy.FirstName))
            .ForMember(dest => dest.CreatedByLastName, opt => opt.MapFrom(src => src.CreatedBy.LastName))
            .ReverseMap()
            .ForMember(dest => dest.Role, opt => opt.Ignore())
            .ForMember(dest => dest.Module, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

        // UserModulePermission ? UserModulePermissionDto
        CreateMap<UserModulePermission, UserModulePermissionDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.ModuleName, opt => opt.MapFrom(src => src.Module.Name))
            .ForMember(dest => dest.ModuleTypeName, opt => opt.MapFrom(src => src.Module.ModuleType.Name))
            .ForMember(dest => dest.CreatedByUsername, opt => opt.MapFrom(src => src.CreatedBy.Username))
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.MapFrom(src => src.CreatedBy.FirstName))
            .ForMember(dest => dest.CreatedByLastName, opt => opt.MapFrom(src => src.CreatedBy.LastName))
            .ReverseMap()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Module, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
    }
}