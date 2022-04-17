using System.Net;
using System.Text.Json.Serialization;

namespace PokeTranslator.Model;

public record PokemonResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("habitat")] string Habitat,
    [property: JsonPropertyName("is_legendary")] bool IsLegendary);

public record Success(int Total);

public record Contents(string Translated, string Text, string Translation);

public record TranslationResponse( Success Success, Contents Contents);

public record HttpResult<T>(bool Success, T? Content, HttpStatusCode StatusCode);
