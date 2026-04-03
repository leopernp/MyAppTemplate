using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Contract.Helpers;

namespace MyAppTemplate.Contract.Interfaces;

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
