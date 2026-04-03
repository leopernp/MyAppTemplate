using AutoMapper;
using AutoMapper.QueryableExtensions;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Helpers;
using DamayanFS.Contract.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DamayanFS.Data.Repositories.Settings.User;

public class UserRepository : BaseRepository<ContextModels.User>, IUserRepository
{
    public UserRepository(Context.ApplicationDbContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    #region Read Methods

    public async Task<IEnumerable<UserDto>> InquireAsync(bool? isActive = null, int? roleId = null)
    {
        var query = _context.Users.AsNoTracking();

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        if (roleId.HasValue && roleId.Value > 0)
            query = query.Where(u => u.RoleId == roleId.Value);

        return await query
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.Username.ToLower() == username.ToLower())
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(x => x.Email.ToLower() == email.ToLower())
            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<CustomValidateResult> ValidateAsync(UserDto user)
    {
        var result = new CustomValidateResult(true);

        // Logic: Find if another user (different ID) has this username
        var checkByUsername = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username.ToLower() == user.Username.ToLower() && x.Id != user.Id);

        if (checkByUsername != null)
            result.AddError("Username is already taken by another user.");

        var checkByEmail = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email.ToLower() == user.Email.ToLower() && x.Id != user.Id);

        if (checkByEmail != null)
            result.AddError("Email is already registered to another account.");

        return result;
    }

    public async Task<UserDto?> GetByUsernameForAuthAsync(string username)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Username.ToLower() == username.ToLower());

        if (entity == null) return null;

        // Manual mapping — bypasses AutoMapper Password ignore
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Password = entity.Password,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            MiddleName = entity.MiddleName,
            Email = entity.Email,
            RoleId = entity.RoleId,
            RoleName = entity.Role?.Name,
            IsActive = entity.IsActive,
            IsSuperAdmin = entity.IsSuperAdmin,
            FailedLoginAttempts = entity.FailedLoginAttempts,
            LockoutEnd = entity.LockoutEnd,
            CreatedById = entity.CreatedById,
            CreatedDate = entity.CreatedDate,
            ModifiedById = entity.ModifiedById,
            ModifiedDate = entity.ModifiedDate
        };
    }

    #endregion

    #region Operation Methods

    public async Task<UserDto> CreateAsync(UserDto dto, int? currentUserId)
    {
        var entity = _mapper.Map<ContextModels.User>(dto);

        entity.CreatedById = currentUserId;
        entity.CreatedDate = DateTime.UtcNow;

        var savedEntity = await CreateWithRollbackAsync(entity);
        return _mapper.Map<UserDto>(savedEntity);
    }

    public async Task<UserDto> UpdateAsync(UserDto dto, int? currentUserId)
    {
        var existing = await _context.Users.FindAsync(dto.Id);
        if (existing == null) throw new Exception("User not found");

        // Map DTO to Entity (Profile.Ignore handles the password safety)
        _mapper.Map(dto, existing);

        existing.ModifiedById = currentUserId;
        existing.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDto>(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdatePasswordAsync(int userId, string hashedPassword)
    {
        var entity = await _context.Users.FindAsync(userId);
        if (entity == null) throw new Exception("User not found.");

        entity.Password = hashedPassword;
        entity.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task UpdateLockoutAsync(int userId, int failedAttempts, DateTime? lockoutEnd)
    {
        var entity = await _context.Users.FindAsync(userId);
        if (entity == null) return;

        entity.FailedLoginAttempts = failedAttempts;
        entity.LockoutEnd = lockoutEnd;

        await _context.SaveChangesAsync();
    }

    public async Task UnlockAsync(int userId)
    {
        var entity = await _context.Users.FindAsync(userId);
        if (entity == null) return;

        entity.FailedLoginAttempts = 0;
        entity.LockoutEnd = null;

        await _context.SaveChangesAsync();
    }

    #endregion
}
