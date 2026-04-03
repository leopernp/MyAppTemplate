using AutoMapper;
using AutoMapper.QueryableExtensions;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Helpers;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DamayanFS.Data.Repositories.Settings;

public class ModuleTypeRepository : BaseRepository<ContextModels.ModuleType>, IModuleTypeRepository
{
    public ModuleTypeRepository(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    #region Read Methods

    public async Task<ModuleTypeDto?> GetByIdAsync(int id)
    {
        return await _context.ModuleTypes
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<ModuleTypeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<ModuleTypeDto?> GetByNameAsync(string name)
    {
        return await _context.ModuleTypes
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<ModuleTypeDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ModuleTypeDto>> InquireAsync(bool? isActive = null)
    {
        var query = _context.ModuleTypes.AsNoTracking();

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        return await query
            .OrderBy(x => x.DisplayOrder)
            .ProjectTo<ModuleTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<IEnumerable<ModuleTypeDto>> GetMenuTreeAsync()
    {
        // Load active ModuleTypes with their active Modules for menu tree assembly
        // Tree is built in memory by the service layer
        return await _context.ModuleTypes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ProjectTo<ModuleTypeDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    #endregion

    #region Validation

    public async Task<CustomValidateResult> ValidateAsync(ModuleTypeDto dto)
    {
        var result = new CustomValidateResult(true);

        // Unique name check
        var existingByName = await _context.ModuleTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name.ToLower() == dto.Name.ToLower() && x.Id != dto.Id);

        if (existingByName != null)
            result.AddError("Module type name already exists.");

        // Controller/Action pair check
        bool hasController = !string.IsNullOrEmpty(dto.Controller);
        bool hasAction = !string.IsNullOrEmpty(dto.Action);

        if (hasController && !hasAction)
            result.AddError("Action is required when Controller is provided.");

        if (hasAction && !hasController)
            result.AddError("Controller is required when Action is provided.");

        // Delete guard — check for active child modules
        if (dto.Id > 0)
        {
            bool hasModules = await _context.Modules
                .AsNoTracking()
                .AnyAsync(x => x.ModuleTypeId == dto.Id);

            if (hasModules)
                result.AddError("Module type cannot be deleted while it has associated modules.");
        }

        return result;
    }

    #endregion

    #region Operation Methods

    public async Task<ModuleTypeDto> CreateAsync(ModuleTypeDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.ModuleType>(dto);
        entity.CreatedById = currentUserId;
        entity.CreatedDate = DateTime.UtcNow;

        var savedEntity = await CreateWithRollbackAsync(entity)
            ?? throw new InvalidOperationException("Failed to create module type.");

        return _mapper.Map<ModuleTypeDto>(savedEntity);
    }

    public async Task<ModuleTypeDto> UpdateAsync(ModuleTypeDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.ModuleType>(dto);
        entity.ModifiedById = currentUserId;
        entity.ModifiedDate = DateTime.UtcNow;

        string[] excludedProperties = new[] { "CreatedById", "CreatedDate" };
        var savedEntity = await UpdateWithRollbackAsync(entity, excludedProperties);

        return _mapper.Map<ModuleTypeDto>(savedEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.ModuleTypes.FindAsync(id);
        if (entity != null)
            await DeleteAsync(entity);
    }

    #endregion
}