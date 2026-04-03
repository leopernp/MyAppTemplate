using DamayanFS.Contract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DamayanFS.App.ApiControllers.Tools
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemInfoController : ControllerBase
    {
        private readonly ISystemInfoService _systemService;

        public SystemInfoController(ISystemInfoService systemService)
        {
            _systemService = systemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfo()
        {
            var info = await _systemService.GetSystemMetricsAsync();
            return Ok(info);
        }
    }
}
