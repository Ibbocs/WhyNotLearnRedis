using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace PubSub.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private ConnectionMultiplexer connectionMultiplexer;

        [HttpGet]
        public async Task<IActionResult> Pub()
        {
            connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
            ISubscriber subscriber = connectionMultiplexer.GetSubscriber();

            long client = 0;
            for (int i = 0; i < 10; i++)
            {
                string message = $"Salam {i}";
                await subscriber.PingAsync();
                client = await subscriber.PublishAsync("myChanell", message);
            }

            return Ok(client);
        }

        [HttpGet]
        public async Task<IActionResult> Sub()
        {
            //bulari ferqli iki proqramda elmeliyem ki pub islediyim anda sub dusun. Cli ile baglanib sub olub gore bilersen
            connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
            ISubscriber subscriber = connectionMultiplexer.GetSubscriber();

            string messageMy = string.Empty;
            await subscriber.SubscribeAsync("myChanell", (channel, message) =>
            {
                messageMy = message;
            });

            return Ok(messageMy);
        }
    }
}
