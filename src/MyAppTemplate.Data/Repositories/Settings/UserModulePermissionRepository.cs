using AutoMapper;
using AutoMapper.QueryableExtensions;
using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MyAppTemplate.Data.Repositories.Settings;

public class UserModulePermissionRepository : BaseRepository<ContextModels.UserModulePermission>, IUserModulePermissionRepository
{
    public UserModulePermissionRepository(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    public async Task<UserModulePermissionDto?> GetUserPermissionAsync(int userId, int moduleId)
    {
        return await _context.UserModulePermissions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.ModuleId == moduleId)
            .ProjectTo<UserModulePermissionDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<UserModulePermissionDto>> GetUserPermissionsAsync(int userId)
    {
        return await _context.UserModulePermissions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.Module.ModuleType.DisplayOrder)
            .ThenBy(x => x.Module.DisplayOrder)
            .ProjectTo<UserModulePermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<UserModulePermissionDto>> GetUserPermissionsByUserAsync(int userId)
    {
        return await _context.UserModulePermissions
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .ProjectTo<UserModulePermissionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<UserModulePermissionDto> SaveUserPermissionAsync(UserModulePermissionDto dto, int? currentUserId)
    {
        var existing = await _context.UserModulePermissions
            .FirstOrDefaultAsync(x => x.UserId == dto.UserId && x.ModuleId == dto.ModuleId);

        if (existing != null)
            await DeleteAsync(existing);

        var entity = _mapper.Map<ContextModels.UserModulePermission>(dto);
        entity.CreatedById = currentUserId;
        entity.CreatedDate = DateTime.UtcNow;

        var savedEntity = await CreateWithRollbackAsync(entity)
            ?? throw new InvalidOperationException("Failed to save user module permission.");

        return _mapper.Map<UserModulePermissionDto>(savedEntity);
    }

    public async Task DeleteUserPermissionAsync(int userId, int moduleId)
    {
        var entity = await _context.UserModulePermissions
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ModuleId == moduleId);

        if (entity != null)
            await DeleteAsync(entity);
    }
}