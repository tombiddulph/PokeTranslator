using System.ComponentModel.DataAnnotations;

namespace PokeTranslator.Config;

public class PokemonServiceOptions
{
    public const string Name = "PokemonService";
    [Url, Required]
    public string PokemonApi { get; set; }
    public TranslationOptions Translations { get; set; }
    
}

public class TranslationOptions
{
    [Url, Required]
    public string Yoda { get; set; }
    [Url, Required]
    public string  Shakespeare { get; set; }
}