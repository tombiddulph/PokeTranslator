using System.Text.Json;
using O9d.Json.Formatting;
using PokeTranslator.Config;
using PokeTranslator.Model;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PokeTranslator.Services;

public interface IPokemonService
{
    Task<HttpResult<PokemonResponse>> GetAsync(string name);
    Task<HttpResult<PokemonResponse>> TranslateAsync(string name);
}

public class PokemonService : IPokemonService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly HttpClient _pokemonApiClient;


    public PokemonService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = Log.Logger.ForContext<PokemonService>() ?? throw new ArgumentException(nameof(Log.Logger));
        _pokemonApiClient = httpClientFactory.CreateClient(nameof(PokemonServiceOptions.PokemonApi)) ??
                            throw new ArgumentException(PokemonServiceOptions.Name);
    }

    public async Task<HttpResult<PokemonResponse>> GetAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var response = await _pokemonApiClient.GetAsync(name);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Warning("Failed to get pokemon {name}", name);
            return new HttpResult<PokemonResponse>(false, null, response.StatusCode);
        }
        

        var pokemon =
           await (JsonSerializer.DeserializeAsync<PokemonSpecies>(await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(),
                   
                }))!;


        return new HttpResult<PokemonResponse>(
            true,
            new PokemonResponse(
                pokemon.Name,
                pokemon.FlavorTextEntries.FirstOrDefault(x => x.Language.Name == "en")?.FlavorText!,
                pokemon.Habitat.Name,
                pokemon.IsLegendary), response.StatusCode);
    }

    public async Task<HttpResult<PokemonResponse>> TranslateAsync(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        var getResponse = await GetAsync(name);

        if (!getResponse.Success)
        {
            return getResponse;
        }

        var pokemon = getResponse.Content!;

        var translator = pokemon.Habitat.ToLower() == "cave" || pokemon.IsLegendary
            ? nameof(TranslationOptions.Yoda)
            : nameof(TranslationOptions.Shakespeare);

        var text = pokemon.Description;

        var client = _httpClientFactory.CreateClient(translator);
        var response = await client.PostAsJsonAsync(string.Empty, new {text});

        if (!response.IsSuccessStatusCode)
        {
            return new HttpResult<PokemonResponse>(false, default, response.StatusCode);
        }

        var translation = await response.Content.ReadFromJsonAsync<TranslationResponse>();

        if (translation?.Success.Total == 0)
        {
            return getResponse;
        }

        var translatedDescription = translation!.Contents.Translated;

        var translatedPokemon = new PokemonResponse(
            pokemon.Name,
            translatedDescription,
            pokemon.Habitat,
            pokemon.IsLegendary);

        return new HttpResult<PokemonResponse>(true, translatedPokemon, response.StatusCode);
    }
}