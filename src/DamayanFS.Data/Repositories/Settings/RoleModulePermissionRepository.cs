using AutoMapper;
using AutoMapper.QueryableExtensions;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DamayanFS.Data.Repositories.Settings;

public class RoleModulePermissionRepository : BaseRepository<ContextModels.RoleModulePermission>, IRoleModulePermissionRepository
{
    public RoleModulePermissionRepository(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    public async Task<RoleModulePermissionDto?> GetRolePermissionAsync(int roleId, int moduleId)
    {
        return await _context.RoleModulePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId && x.ModuleId == moduleId)
            .ProjectTo<RoleModulePermissionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<RoleModulePermissionDto>> GetRolePermissionsAsync(int roleId)
    {
        return await _context.RoleModulePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .OrderBy(x => x.Module.ModuleType.DisplayOrder)
            .ThenBy(x => x.Module.DisplayOrder)
            .ProjectTo<RoleModulePermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<RoleModulePermissionDto>> GetRolePermissionsByRoleAsync(int roleId)
    {
        return await _context.RoleModulePermissions
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .ProjectTo<RoleModulePermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<RoleModulePermissionDto> SaveRolePermissionAsync(RoleModulePermissionDto dto, int? currentUserId)
    {
        var existing = await _context.RoleModulePermissions
            .FirstOrDefaultAsync(x => x.RoleId == dto.RoleId && x.ModuleId == dto.ModuleId);

        if (existing != null)
            await DeleteAsync(existing);

        var entity = _mapper.Map<ContextModels.RoleModulePermission>(dto);
        entity.CreatedById = currentUserId;
        entity.CreatedDate = DateTime.UtcNow;

        var savedEntity = await CreateWithRollbackAsync(entity)
            ?? throw new InvalidOperationException("Failed to save role module permission.");

        return _mapper.Map<RoleModulePermissionDto>(savedEntity);
    }

    public async Task DeleteRolePermissionAsync(int roleId, int moduleId)
    {
        var entity = await _context.RoleModulePermissions
            .FirstOrDefaultAsync(x => x.RoleId == roleId && x.ModuleId == moduleId);

        if (entity != null)
            await DeleteAsync(entity);
    }
}