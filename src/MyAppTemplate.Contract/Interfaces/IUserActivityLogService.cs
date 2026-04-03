using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Contract.Enums;

namespace MyAppTemplate.Contract.Interfaces;

public interface IUserActivityLogService
{
    Task LogAsync(UserActivityAction action, string description, int? performedById, string? ipAddress, string? browser);
    Task DeleteAsync(int id);
    Task<IEnumerable<UserActivityLogDto>> InquireAsync(int? performedById = null, UserActivityAction? action = null);
    Task<UserActivityLogDto?> GetByIdAsync(int id);
}
