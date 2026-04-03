using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Enums;

namespace DamayanFS.Contract.Interfaces;

public interface IUserActivityLogRepository
{
    Task<UserActivityLogDto> CreateAsync(UserActivityLogDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<UserActivityLogDto>> InquireAsync(int? performedById = null, UserActivityAction? action = null);
    Task<UserActivityLogDto?> GetByIdAsync(int id);
}
