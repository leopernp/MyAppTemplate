using AutoMapper;
using DamayanFS.Data.Context;

namespace DamayanFS.Data.Repositories;

public class BaseRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly IMapper _mapper;

    #region Constructor

    public BaseRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #endregion

    #region Operations

    public async Task<TEntity?> CreateWithRollbackAsync(TEntity entity)
    {
        return await CreateAsync(entity, true);
    }

    public async Task<TEntity> CreateNoRollbackAsync(TEntity entity)
    {
        return await CreateAsync(entity, false) ?? throw new InvalidOperationException("Creation failed unexpectedly.");
    }

    // Base create method to be used internally
    private async Task<TEntity?> CreateAsync(TEntity entity, bool withRollback = true)
    {
        if (withRollback)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Set<TEntity>().AddAsync(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return entity;
            }
            catch
            {
                await transaction.RollbackAsync();
                return null;
            }
        }
        else
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }

    public async Task<TEntity> UpdateNoRollbackAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateWithRollbackAsync(TEntity entity, params string[] excludedProperties)
    {
        return await UpdateAsync(entity, true, excludedProperties);
    }

    public async Task<TEntity> UpdateWithoutRollbackAsync(TEntity entity, params string[] excludedProperties)
    {
        return await UpdateAsync(entity, false, excludedProperties);
    }

    // Base update method to be used internally
    private async Task<TEntity> UpdateAsync(TEntity entity, bool withRollback = true, params string[] excludedProperties)
    {
        if (withRollback)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Set<TEntity>().Update(entity);

                foreach (var property in excludedProperties)
                {
                    _context.Entry(entity).Property(property).IsModified = false;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return entity;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        else
        {
            _context.Set<TEntity>().Update(entity);

            foreach (var property in excludedProperties)
            {
                _context.Entry(entity).Property(property).IsModified = false;
            }

            await _context.SaveChangesAsync();
            return entity;
        }
    }

    public async Task<bool> DeleteAsync(TEntity entity)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> BulkDeleteAsync(IEnumerable<TEntity> entities)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Set<TEntity>().RemoveRange(entities);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    #endregion
}
