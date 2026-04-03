using AutoMapper;
using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Contract.Models.Module;
using MyAppTemplate.Contract.Models.ModulePermission;
using MyAppTemplate.Contract.Models.ModuleType;
using MyAppTemplate.Contract.Models.Role;
using MyAppTemplate.Contract.Models.User;

namespace MyAppTemplate.App.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        // User
        CreateMap<UserUpsertModel, UserDto>();

        // Role
        CreateMap<RoleUpsertModel, RoleDto>();

        // ModuleType
        CreateMap<ModuleTypeUpsertModel, ModuleTypeDto>()
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUsername, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByLastName, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedByUsername, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedByFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedByLastName, opt => opt.Ignore())
            .ForMember(dest => dest.Modules, opt => opt.Ignore());

        // Module
        CreateMap<ModuleUpsertModel, ModuleDto>()
            .ForMember(dest => dest.ModuleTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.ParentModuleName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedById, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUsername, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByLastName, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedByUsername, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedByFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.ModifiedByLastName, opt => opt.Ignore())
            .ForMember(dest => dest.ChildModules, opt => opt.Ignore())
            .ForMember(dest => dest.CanView, opt => opt.Ignore())
            .ForMember(dest => dest.CanCreate, opt => opt.Ignore())
            .ForMember(dest => dest.CanUpdate, opt => opt.Ignore())
            .ForMember(dest => dest.CanDelete, opt => opt.Ignore())
            .ForMember(dest => dest.IsAccessible, opt => opt.Ignore());

        // RoleModulePermission
        CreateMap<RoleModulePermissionUpsertModel, RoleModulePermissionDto>()
            .ForMember(dest => dest.RoleName, opt => opt.Ignore())
            .ForMember(dest => dest.ModuleName, opt => opt.Ignore())
            .ForMember(dest => dest.ModuleTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUsername, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByLastName, opt => opt.Ignore());

        // UserModulePermission
        CreateMap<UserModulePermissionUpsertModel, UserModulePermissionDto>()
            .ForMember(dest => dest.Username, opt => opt.Ignore())
            .ForMember(dest => dest.UserFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.UserLastName, opt => opt.Ignore())
            .ForMember(dest => dest.ModuleName, opt => opt.Ignore())
            .ForMember(dest => dest.ModuleTypeName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByUsername, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByFirstName, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByLastName, opt => opt.Ignore());
    }
}