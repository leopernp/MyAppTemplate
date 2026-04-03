using DamayanFS.Contract.DTO.Tools;

namespace DamayanFS.Contract.Interfaces;

public interface ISystemInfoService
{
    Task<SystemStatusDto> GetSystemMetricsAsync();
}
