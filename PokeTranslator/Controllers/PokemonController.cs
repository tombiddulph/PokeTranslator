using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;


namespace PokeTranslator.Controllers;

[ApiController]
[Route("[controller]")]
public class PokemonController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IPokemonService _pokemonService;

    public PokemonController(ILogger logger, IPokemonService pokemonService)
    {
        _logger = logger.ForContext<PokemonController>() ?? throw new ArgumentNullException(nameof(logger));
        _pokemonService = pokemonService ?? throw new ArgumentNullException(nameof(pokemonService));
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<object> Get(string name)
    {
        _logger.Information("Getting pokemon {name}", name);
        var pokemon = await _pokemonService.GetAsync(name);
        return pokemon;
    }
    
}