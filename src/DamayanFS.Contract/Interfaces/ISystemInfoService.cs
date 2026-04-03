using MyAppTemplate.Contract.DTO.Tools;

namespace MyAppTemplate.Contract.Interfaces;

public interface ISystemInfoService
{
    Task<SystemStatusDto> GetSystemMetricsAsync();
}
