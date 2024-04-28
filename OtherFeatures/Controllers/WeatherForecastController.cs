using Microsoft.AspNetCore.Mvc;

namespace OtherFeatures.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IActionResult> Get()
        {
            RedisPipelining redisPipelining = new RedisPipelining();

            var res = await redisPipelining.Pipelining();
            return Ok(res);
        }
    }
}
