using AutoMapper;
using AutoMapper.QueryableExtensions;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Enums;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DamayanFS.Data.Repositories.Settings
{
    public class UserActivityLogRepository : BaseRepository<ContextModels.UserActivityLog>, IUserActivityLogRepository
    {
        public UserActivityLogRepository(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        #region Read Methods

        public async Task<UserActivityLogDto?> GetByIdAsync(int id)
        {
            return await _context.UserActivityLogs
                .AsNoTracking()
                .Where(x => x.Id == id)
                .ProjectTo<UserActivityLogDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserActivityLogDto>> InquireAsync(
            int? performedById = null,
            UserActivityAction? action = null)
        {
            var query = _context.UserActivityLogs
                .AsNoTracking()
                .AsQueryable();

            if (performedById.HasValue)
                query = query.Where(x => x.PerformedById == performedById);

            if (action.HasValue)
                query = query.Where(x => x.Action == action);

            return await query
                .OrderByDescending(x => x.Timestamp)
                .ProjectTo<UserActivityLogDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        #endregion

        #region Operation Methods

        public async Task<UserActivityLogDto> CreateAsync(UserActivityLogDto dto)
        {
            var entity = _mapper.Map<ContextModels.UserActivityLog>(dto);
            entity.Timestamp = DateTime.UtcNow;

            var savedEntity = await CreateWithRollbackAsync(entity);
            return _mapper.Map<UserActivityLogDto>(savedEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.UserActivityLogs.FindAsync(id);
            if (entity != null)
            {
                _context.UserActivityLogs.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
