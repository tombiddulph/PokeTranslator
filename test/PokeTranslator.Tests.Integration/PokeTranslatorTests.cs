using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace PokeTranslator.Tests.Integration;

[UsesVerify]
public class PokeTranslatorTests : IClassFixture<PokeTranslator>
{
    protected readonly PokeTranslator Translator;
    private static readonly VerifySettings _verifierSettings = new();

    public PokeTranslatorTests(PokeTranslator translator)
    {
        Translator = translator;
        _verifierSettings.UseDirectory("Snapshots");
        _verifierSettings.ScrubInlineGuids();
    }

    [Fact]
    public async Task GetAsync_Returns_Pokemon_When_Pokemon_Found()
    {
        //arrange
        var client = Translator.CreateClient();

        //act
        var response = await client.GetAsync("pokemon/mewtwo");
        var content = await response.Content.ReadAsStringAsync();

        //assert
        await Verifier.Verify(new {message = response, content}, _verifierSettings);
    }

    [Fact]
    public async Task GetAsync_Returns_404_When_Pokemon_Not_Found()
    {
        //arrange
        var client = Translator.CreateClient();

        //act
        var response = await client.GetAsync("pokemon/tomb");
        var content = await response.Content.ReadAsStringAsync();

        //assert
        await Verifier.Verify(new {message = response, content}, _verifierSettings);
    }
    
    [Fact]
    public async Task TranslateAsync_Returns_Yoda_Translated_Description_When_Pokemon_Found()
    {
        //arrange
        var client = Translator.CreateClient();

        //act
        var response = await client.GetAsync("pokemon/translated/mewtwo");
        var content = await response.Content.ReadAsStringAsync();

        //assert
        await Verifier.Verify(new {message = response, content}, _verifierSettings);
    }

    [Fact]
    public async Task TranslateAsync_Returns_Normal_Description_When_Translation_Fails()
    {
        //arrange
        var client = Translator.CreateClient();

        //act
        var response = await client.GetAsync("pokemon/translated/bulbasaur");
        var content = await response.Content.ReadAsStringAsync();

        //assert
        await Verifier.Verify(new {message = response, content}, _verifierSettings);
    }
}