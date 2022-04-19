namespace PokeTranslator.Model;

public record Language(string Name);

public record FlavorTextEntry(string FlavorText, Language Language);

public record Habitat(string Name);

public record PokemonSpecies(
    int BaseHappiness,
    int CaptureRate,
    IReadOnlyList<FlavorTextEntry> FlavorTextEntries,
    bool FormsSwitchable,
    int GenderRate,
    Habitat Habitat,
    bool HasGenderDifferences,
    int HatchCounter,
    bool IsBaby,
    bool IsLegendary,
    bool IsMythical,
    string Name,
    int Order
);