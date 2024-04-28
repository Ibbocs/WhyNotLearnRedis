using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace In_MemoryCache.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private IMemoryCache _memoryCache;

        public WeatherForecastController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public IActionResult SetChache()
        {
            var result = _memoryCache.Set("name", "ibbocs"); //bu result elave elediyi data'ni donur geriye
            var result2 = _memoryCache.Set("name2", "ibbocs2",DateTimeOffset.UtcNow.AddSeconds(5)); //expaird vaxtdi verdim

            //var key = _memoryCache.CreateEntry("viyyui"); //todo ne olduguna sora baxacam
            //key.SetValue("baki");
            //key.SetOptions(new()
            //{

            //});

            //key.

            var result3 = _memoryCache.Set<string>("name", "ibbocs3");

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetChache()
        {
            var result1 = _memoryCache.Get<string>("name"); //object kimi gelmesin, verdiyim tipde gelsin deye
            var result = _memoryCache.Get<string>("name2"); //object kimi gelmesin, verdiyim tipde gelsin deye

            return Ok(result);
        }

        [HttpGet]
        public IActionResult TryGetChache()
        {
            var result = string.Empty;

            if (_memoryCache.TryGetValue<string>("name", out string nameValue)) //TryParse kimi eyni mentiqdedi.
            {
                result += $" {nameValue}";
            }

            if (_memoryCache.TryGetValue<string>("name2", out string name2Value))
            {
                result += $" {name2Value}";
            }

            if (_memoryCache.TryGetValue<DateTime>("date", out DateTime dateValue))
            {
                result += $" {dateValue}";
            }

            return Ok(result);
        }

        [HttpGet]
        public IActionResult SetWithOptionChache()
        {
            //qeydelerimde var bularin ne oldugu
            var result = _memoryCache.Set<DateTime>("date", DateTime.Now, options: new()
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30),
                SlidingExpiration = TimeSpan.FromSeconds(10),
            });

            //var options = new MemoryCacheEntryOptions
            //{
            //    AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30),
            //    SlidingExpiration = TimeSpan.FromSeconds(10),
            //    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5),
            //    Size = 10,
            //    Priority = CacheItemPriority.Normal,
            //    PostEvictionCallbacks = new[] { new PostEvictionCallbackRegistration { EvictionCallback = OnEviction } },
            //    ExpirationTokens = new[] { new CancellationChangeToken(new System.Threading.CancellationToken()) }
            //};

            return Ok(result);
        }
    }
}
