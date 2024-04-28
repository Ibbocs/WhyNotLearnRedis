using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace DistributedCache.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly IDistributedCache _distributedCache;

        public WeatherForecastController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> Set()
        {
            await _distributedCache.SetStringAsync("name1", "ibbocs");

            await _distributedCache.SetStringAsync("name", "ibbocs2", options: new()
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(1),
                SlidingExpiration = TimeSpan.FromSeconds(20)
            });

            _distributedCache.Set("surname", Encoding.UTF8.GetBytes("baki"));

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var name = await _distributedCache.GetStringAsync("name");
            var binarySurname = await _distributedCache.GetAsync("surname");
            var surname = Encoding.UTF8.GetString(binarySurname);

            return Ok(new
            {
                name,
                surname
            });
        }
    }
}
