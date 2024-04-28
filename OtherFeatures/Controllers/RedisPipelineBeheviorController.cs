using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace OtherFeatures.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RedisPipelineBeheviorController : ControllerBase
{
    //qeyd bu RedisPipelining ile eyni sey deyil, bu beheviordu elave edirik ve isarelediyimiz yerleri cache elave edir cqrs-medaitor patterni ile isledirk, ordaki ise sorgulari yigib tek seferde gondermek ucun idi.
    private IMediator? _mediator;

    public RedisPipelineBeheviorController(IMediator? mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var response = await _mediator.Send(new GetAllProductsQueryRequest());

        return Ok(response);
    }
}

public interface IRedisCacheService
{
    Task<T> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, DateTime? expire = null);
}

public class RedisCacheService : IRedisCacheService
{
    private readonly ConnectionMultiplexer _redisCacheConnection;
    private readonly IDatabase _database;
    private readonly RedisCacheSettings _redisCacheSettings;

    public RedisCacheService(IOptions<RedisCacheSettings> options)
    {
        _redisCacheSettings = options.Value;//program.cs de elave elemek lazim ola biler
        var opt = ConfigurationOptions.Parse(_redisCacheSettings.ConnectionString);
        _redisCacheConnection = ConnectionMultiplexer.Connect(opt);
        _database = _redisCacheConnection.GetDatabase();
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);
        if (value.HasValue)
            return JsonConvert.DeserializeObject<T>(value);

        return default;
    }

    public async Task SetAsync<T>(string key, T value, DateTime? expire = null)
    {
        TimeSpan timeUnitExpiration = expire.Value - DateTime.Now;
        await _database.StringSetAsync(key, JsonConvert.SerializeObject(value), timeUnitExpiration);
    }
}

public class RedisCacheSettings
{
    public string Instance { get; set; }//database name
    public string ConnectionString { get; set; }
}

//mediator behavioru
public class RedisCacheBehevior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IRedisCacheService redisCacheService;

    public RedisCacheBehevior(IRedisCacheService redisCacheService)
    {
        this.redisCacheService = redisCacheService;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ICacheableQuery query)
        {
            var cacheKey = query.CacheKey;
            var cacheTime = query.CacheTime;

            var cachedData = await redisCacheService.GetAsync<TResponse>(cacheKey);
            if (cachedData is not null)
                return cachedData;

            var response = await next();
            if (response is not null)
                await redisCacheService.SetAsync(cacheKey, response, DateTime.Now.AddMinutes(cacheTime));

            return response;
        }

        return await next();
    }
}

//neleri cach eleyeceyikse oralarda bunla bildireceyik
public interface ICacheableQuery
{
    string CacheKey { get; }
    double CacheTime { get; }
}

public class GetAllProductsQueryRequest : IRequest<IList<GetAllProductsQueryResponse>>, ICacheableQuery
{
    public string CacheKey => "GetAllProducts";

    public double CacheTime => 60;
}

public class GetAllProductsQueryResponse
{
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQueryRequest, IList<GetAllProductsQueryResponse>>
{

    public async Task<IList<GetAllProductsQueryResponse>> Handle(GetAllProductsQueryRequest request, CancellationToken cancellationToken)
    {
        List<GetAllProductsQueryResponse> responses = new List<GetAllProductsQueryResponse>()
        {
         new GetAllProductsQueryResponse()
         {
             Description ="ssDes",
             Title ="ssTit",
             Price = 1,
             Discount =12
         },

         new GetAllProductsQueryResponse()
         {
             Description ="ssDes",
             Title ="ssTit",
             Price = 1,
             Discount =12
         }
        };

        return responses;
    }
}