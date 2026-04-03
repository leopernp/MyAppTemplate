using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Helpers;

namespace DamayanFS.Contract.Interfaces;

public interface IRoleRepository
{
    Task<RoleDto> CreateAsync(RoleDto dto, int? currentUserId);
    Task DeleteAsync(int id);
    Task<RoleDto?> GetByIdAsync(int id);
    Task<RoleDto?> GetByNameAsync(string name);
    Task<IEnumerable<RoleDto>> InquireAsync(bool? isActive = null);
    Task<RoleDto> UpdateAsync(RoleDto dto, int? currentUserId);
    Task<CustomValidateResult> ValidateAsync(RoleDto role);
}
