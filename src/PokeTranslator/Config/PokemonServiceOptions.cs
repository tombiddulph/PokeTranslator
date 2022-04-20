using System.ComponentModel.DataAnnotations;

namespace PokeTranslator.Config;

public class PokemonServiceOptions
{
    public const string Name = "PokemonService";
    [Url, Required]
    public string PokemonApi { get; set; } = null!;

    public TranslationOptions Translations { get; set; } = null!;
    
}

public class TranslationOptions
{
    [Url, Required]
    public string Yoda { get; set; } = null!;
    [Url, Required]
    public string  Shakespeare { get; set; } = null!;
}