using AutoMapper;
using AutoMapper.QueryableExtensions;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Helpers;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DamayanFS.Data.Repositories.Settings;

public class ModuleRepository : BaseRepository<ContextModels.Module>, IModuleRepository
{
    public ModuleRepository(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    #region Read Methods

    public async Task<ModuleDto?> GetByIdAsync(int id)
    {
        return await _context.Modules
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<ModuleDto?> GetByNameAsync(string name)
    {
        return await _context.Modules
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ModuleDto>> InquireAsync(
        bool? isActive = null,
        int? moduleTypeId = null,
        int? parentModuleId = null)
    {
        var query = _context.Modules.AsNoTracking();

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (moduleTypeId.HasValue)
            query = query.Where(x => x.ModuleTypeId == moduleTypeId.Value);

        if (parentModuleId.HasValue)
            query = query.Where(x => x.ParentModuleId == parentModuleId.Value);

        return await query
            .OrderBy(x => x.ModuleTypeId)
            .ThenBy(x => x.DisplayOrder)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<ModuleDto>> GetByModuleTypeAsync(int moduleTypeId)
    {
        return await _context.Modules
            .AsNoTracking()
            .Where(x => x.ModuleTypeId == moduleTypeId)
            .OrderBy(x => x.DisplayOrder)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<ModuleDto>> GetChildrenAsync(int parentModuleId)
    {
        return await _context.Modules
            .AsNoTracking()
            .Where(x => x.ParentModuleId == parentModuleId)
            .OrderBy(x => x.DisplayOrder)
            .ProjectTo<ModuleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    #endregion

    #region Validation

    public async Task<CustomValidateResult> ValidateAsync(ModuleDto dto)
    {
        var result = new CustomValidateResult(true);

        // Unique name within the same ModuleType
        var existingByName = await _context.Modules
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.Name.ToLower() == dto.Name.ToLower() &&
                x.ModuleTypeId == dto.ModuleTypeId &&
                x.Id != dto.Id);

        if (existingByName != null)
            result.AddError("Module name already exists within this module type.");

        // Controller/Action pair check
        bool hasController = !string.IsNullOrEmpty(dto.Controller);
        bool hasAction = !string.IsNullOrEmpty(dto.Action);

        if (hasController && !hasAction)
            result.AddError("Action is required when Controller is provided.");

        if (hasAction && !hasController)
            result.AddError("Controller is required when Action is provided.");

        // Circular reference check for ParentModuleId
        if (dto.ParentModuleId.HasValue)
        {
            if (dto.ParentModuleId.Value == dto.Id)
            {
                result.AddError("A module cannot be its own parent.");
            }
            else if (dto.Id > 0)
            {
                bool isCircular = await IsCircularReferenceAsync(dto.Id, dto.ParentModuleId.Value);
                if (isCircular)
                    result.AddError("The selected parent module creates a circular reference.");
            }
        }

        // Delete guard — check for active child modules
        if (dto.Id > 0)
        {
            bool hasChildren = await _context.Modules
                .AsNoTracking()
                .AnyAsync(x => x.ParentModuleId == dto.Id);

            if (hasChildren)
                result.AddError("Module cannot be deleted while it has child modules.");

            bool hasPermissions = await _context.RoleModulePermissions
                .AsNoTracking()
                .AnyAsync(x => x.ModuleId == dto.Id);

            if (!hasPermissions)
            {
                hasPermissions = await _context.UserModulePermissions
                    .AsNoTracking()
                    .AnyAsync(x => x.ModuleId == dto.Id);
            }

            if (hasPermissions)
                result.AddError("Module cannot be deleted while it has active permission assignments.");
        }

        return result;
    }

    #endregion

    #region Operation Methods

    public async Task<ModuleDto> CreateAsync(ModuleDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.Module>(dto);
        entity.CreatedById = currentUserId;
        entity.CreatedDate = DateTime.UtcNow;

        var savedEntity = await CreateWithRollbackAsync(entity)
            ?? throw new InvalidOperationException("Failed to create module.");

        return _mapper.Map<ModuleDto>(savedEntity);
    }

    public async Task<ModuleDto> UpdateAsync(ModuleDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.Module>(dto);
        entity.ModifiedById = currentUserId;
        entity.ModifiedDate = DateTime.UtcNow;

        string[] excludedProperties = new[] { "CreatedById", "CreatedDate" };
        var savedEntity = await UpdateWithRollbackAsync(entity, excludedProperties);

        return _mapper.Map<ModuleDto>(savedEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.Modules.FindAsync(id);
        if (entity != null)
            await DeleteAsync(entity);
    }

    #endregion

    #region Private Helpers

    private async Task<bool> IsCircularReferenceAsync(int moduleId, int parentModuleId)
    {
        // Walk up the ancestor chain from the proposed parent
        // If we encounter the moduleId being edited, it's a circular reference
        var visitedIds = new HashSet<int>();
        int? currentId = parentModuleId;

        while (currentId.HasValue)
        {
            if (visitedIds.Contains(currentId.Value))
                return true; // Cycle detected in existing data

            if (currentId.Value == moduleId)
                return true; // Would create a cycle

            visitedIds.Add(currentId.Value);

            var parent = await _context.Modules
                .AsNoTracking()
                .Where(x => x.Id == currentId.Value)
                .Select(x => x.ParentModuleId)
                .FirstOrDefaultAsync();

            currentId = parent;
        }

        return false;
    }

    #endregion
}