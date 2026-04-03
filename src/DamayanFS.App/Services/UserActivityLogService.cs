using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Enums;
using DamayanFS.Contract.Interfaces;

namespace DamayanFS.App.Services;

public class UserActivityLogService : IUserActivityLogService
{
    private readonly ILogger<UserActivityLogService> _logger;
    private readonly IUserActivityLogRepository _userActivityLogRepository;

    public UserActivityLogService(
        ILogger<UserActivityLogService> logger,
        IUserActivityLogRepository userActivityLogRepository)
    {
        _logger = logger;
        _userActivityLogRepository = userActivityLogRepository;
    }

    public async Task LogAsync(
        UserActivityAction action,
        string description,
        int? performedById,
        string? ipAddress,
        string? browser)
    {
        try
        {
            var dto = new UserActivityLogDto
            {
                Action = action,
                Description = description,
                PerformedById = performedById,
                IpAddress = ipAddress,
                Browser = browser
            };

            await _userActivityLogRepository.CreateAsync(dto);
        }
        catch (Exception ex)
        {
            // Logging failure should never crash the calling operation
            _logger.LogError(ex, "Error occurred while logging user activity. Action: {Action}, PerformedById: {PerformedById}",
                action, performedById);
        }
    }

    public async Task<UserActivityLogDto?> GetByIdAsync(int id) =>
        await _userActivityLogRepository.GetByIdAsync(id);

    public async Task<IEnumerable<UserActivityLogDto>> InquireAsync(
        int? performedById = null,
        UserActivityAction? action = null) =>
        await _userActivityLogRepository.InquireAsync(performedById, action);

    public async Task DeleteAsync(int id) =>
        await _userActivityLogRepository.DeleteAsync(id);
}
