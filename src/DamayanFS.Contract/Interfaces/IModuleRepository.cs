using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Helpers;

namespace DamayanFS.Contract.Interfaces;

public interface IModuleRepository
{
    // Read
    Task<ModuleDto?> GetByIdAsync(int id);
    Task<ModuleDto?> GetByNameAsync(string name);
    Task<IEnumerable<ModuleDto>> InquireAsync(bool? isActive = null, int? moduleTypeId = null, int? parentModuleId = null);
    Task<IEnumerable<ModuleDto>> GetByModuleTypeAsync(int moduleTypeId);
    Task<IEnumerable<ModuleDto>> GetChildrenAsync(int parentModuleId);

    // Validation
    Task<CustomValidateResult> ValidateAsync(ModuleDto dto);

    // Operations
    Task<ModuleDto> CreateAsync(ModuleDto dto, int? currentUserId);
    Task<ModuleDto> UpdateAsync(ModuleDto dto, int? currentUserId);
    Task DeleteAsync(int id);
}