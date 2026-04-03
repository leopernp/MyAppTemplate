using DamayanFS.Contract.DTO;

namespace DamayanFS.Contract.Interfaces;

public interface IMenuCacheService
{
    Task<IEnumerable<ModuleTypeDto>> GetMenuAsync(int userId, int roleId, bool isSuperAdmin = false);
    void InvalidateMenu(int userId);
    void InvalidateAllMenus();
}