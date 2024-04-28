using OtherFeatures.Controllers;
using OtherFeatures.DTOs;
using System.Text.Json;

namespace OtherFeatures.Services;

public class EntityService : IEntityService 
{
    
    private readonly RediseService _rediseService;

    public EntityService(RediseService rediseService)
    {
        _rediseService = rediseService;
    }

    public async Task<bool> Delete(string id)
    {
        var status = await _rediseService.GetDatabase().KeyDeleteAsync(id);
        return status;
    }

    public async Task<EntityDTO> Get(string id)
    {
        var existEntity = await _rediseService.GetDatabase().StringGetAsync(id);
        if (string.IsNullOrEmpty(existEntity))
        {
            //message done bilerik burda yoxdu zad
            return null;
        }

        return JsonSerializer.Deserialize<EntityDTO>(existEntity);
    }

    public async Task<bool> Save(EntityDTO model)
    {
        var status = await _rediseService.GetDatabase().StringSetAsync(model.Id.ToString(), JsonSerializer.Serialize(model));
        return status;
    }
}

public interface IEntityService
{
    public Task<bool> Save(EntityDTO model);
    Task<EntityDTO> Get(string id);
    Task<bool> Delete(string id);
}
