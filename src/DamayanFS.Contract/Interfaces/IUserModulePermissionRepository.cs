using DamayanFS.Contract.DTO;

namespace DamayanFS.Contract.Interfaces;

public interface IUserModulePermissionRepository
{
    Task<UserModulePermissionDto?> GetUserPermissionAsync(int userId, int moduleId);
    Task<IEnumerable<UserModulePermissionDto>> GetUserPermissionsAsync(int userId);
    Task<IEnumerable<UserModulePermissionDto>> GetUserPermissionsByUserAsync(int userId);
    Task<UserModulePermissionDto> SaveUserPermissionAsync(UserModulePermissionDto dto, int? currentUserId);
    Task DeleteUserPermissionAsync(int userId, int moduleId);
}