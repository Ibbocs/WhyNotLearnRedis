using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OtherFeatures.DTOs;
using OtherFeatures.Services;
using StackExchange.Redis;

namespace OtherFeatures.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EntityController : ControllerBase
{
    //burda database kimi isledirik redisi, hansi ki datalarin silinmir, serveri kill elesek bele
    private IEntityService service;

    public EntityController(IEntityService service)
    {
        this.service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string id)
    {
        var result = await service.Get(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Save(EntityDTO model)
    {
        var result = await service.Save(model);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await service.Delete(id);
        return Ok(result);
    }
}

public class RedisSettings
{
    public int Port { get; set; }
    public string Host { get; set; }
}

public class RediseService
{
    public RediseService(string host, int port)
    {
        _port = port;
        _host = host;
    }

    private ConnectionMultiplexer _multiplexer;

    public int _port { get; set; }
    public string _host { get; set; }

    public void Connect() => _multiplexer = ConnectionMultiplexer.Connect($"{_host}:{_port}");
    //int db =1 demeyimin sebebi ozunun icinde default olan db1 secsin, ve icinden ilk getirsin deye 0 yaziram, amma bunu elemeden oz database yaradib getizdire bilerik. O versialari elmisem deye bunu test edirem burda indi.
    public IDatabase GetDatabase(int db = 1) => _multiplexer.GetDatabase(0);

}
