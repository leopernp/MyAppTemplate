using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace DamayanFS.App.ApiControllers.Tools
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheManagementController : ControllerBase
    {
        private readonly IMemoryCache _cache;

        public CacheManagementController(IMemoryCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public IActionResult GetAllKeys()
        {
            var keys = new List<string>();

            if (_cache is MemoryCache concreteCache)
            {
                foreach (var key in concreteCache.Keys)
                {
                    keys.Add(key.ToString());
                }
            }

            return Ok(new
            {
                Count = keys.Count,
                Keys = keys,
                Type = "In-Memory"
            });
        }

        [HttpGet("{key}")]
        public IActionResult GetValue(string key)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return Ok(new { Key = key, Value = value });
            }
            return NotFound(new { Message = $"Key '{key}' not found or expired." });
        }

        [HttpDelete("{key}")]
        public IActionResult Remove(string key)
        {
            _cache.Remove(key);
            return NoContent();
        }

        [HttpDelete]
        public IActionResult ClearAll()
        {
            if (_cache is MemoryCache concreteCache)
            {
                concreteCache.Clear();
                return Ok(new { Message = "Cache cleared successfully." });
            }
            return StatusCode(500, "Could not clear cache.");
        }
    }
}
