using AutoMapper;
using AutoMapper.QueryableExtensions;
using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Contract.Helpers;
using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace MyAppTemplate.Data.Repositories.Settings;

public class RoleRepository : BaseRepository<ContextModels.Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext contxt, IMapper mapper)
        : base(contxt, mapper)
    {
    }

    #region Read Methods

    public async Task<IEnumerable<RoleDto>> InquireAsync(bool? isActive = null)
    {
        var query = _context.Roles.AsNoTracking();

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        return await query
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<RoleDto?> GetByIdAsync(int id)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<RoleDto?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<RoleDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<CustomValidateResult> ValidateAsync(RoleDto role)
    {
        var result = new CustomValidateResult(true);

        var checkyByName = await _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Name.ToLower() == role.Name.ToLower() && r.Id != role.Id);

        if (checkyByName != null)
            result.AddError("Name already exists!");

        return result;
    }

    #endregion

    #region Operation Methods

    public async Task<RoleDto> CreateAsync(RoleDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.Role>(dto);

        entity.CreatedById = currentUserId;
        entity.CreatedDate = DateTime.UtcNow;

        var savedEntity = await CreateWithRollbackAsync(entity);
        return _mapper.Map<RoleDto>(savedEntity);
    }

    public async Task<RoleDto> UpdateAsync(RoleDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.Role>(dto);

        entity.ModifiedById = currentUserId;
        entity.ModifiedDate = DateTime.UtcNow;

        string[] excludedProperties = new[] { "CreatedById", "CreatedDate" };

        var savedEntity = await UpdateWithRollbackAsync(entity, excludedProperties);
        return _mapper.Map<RoleDto>(savedEntity);
    }

    public async Task DeleteAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }

    #endregion
}
