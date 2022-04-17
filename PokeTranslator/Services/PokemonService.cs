using Microsoft.Extensions.Options;
using PokeTranslator.Config;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PokeTranslator;

public interface IPokemonService
{
    Task<object>  GetAsync(string name);
    Task TranslateAsync(string name);
}

public class PokemonService : IPokemonService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<PokemonServiceOptions> _options;
    private readonly ILogger _logger;

    public PokemonService(IHttpClientFactory httpClientFactory, IOptions<PokemonServiceOptions> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = Log.Logger.ForContext<PokemonService>() ?? throw new ArgumentException(nameof(Log.Logger));
    }

    public Task<object> GetAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        _httpClientFactory.CreateClient();
        return default;
    }

    public Task TranslateAsync(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }
        throw new NotImplementedException();
    }
}

