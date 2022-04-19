using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using PokeTranslator.Config;
using PokeTranslator.Services;
using PokeTranslator.Tests.Unit.Helpers;
using Xunit;

namespace PokeTranslator.Tests.Unit;

public class PokemonServiceTests
{
    private IPokemonService? _pokemonService;
    private readonly IHttpClientFactory _httpClientFactory;

    public PokemonServiceTests()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
    }

    [Fact]
    public async Task GetAsync_ReturnsEmptyResult_When_Pokemon_Not_Found()
    {
        //arrange
        _httpClientFactory.CreateClient(Arg.Is(nameof(PokemonServiceOptions.PokemonApi)))
            .Returns(new HttpClient(new MockHttpStatusCodeHandler(HttpStatusCode.NotFound))
                {BaseAddress = new Uri("http://localhost")});

        _pokemonService = new PokemonService(_httpClientFactory);

        //act
        var result = await _pokemonService.GetAsync("tom");

        //assert
        result.Success.Should().BeFalse();
        result.Content.Should().BeNull();
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAsync_ThrowsArgumentNullException_When_Pokemon_Name_Is_Null()
    {
        //arrange
        _httpClientFactory.CreateClient(Arg.Is(nameof(PokemonServiceOptions.PokemonApi)))
            .Returns(new HttpClient(new MockHttpStatusCodeHandler(HttpStatusCode.NotFound))
                {BaseAddress = new Uri("http://localhost")});

        _pokemonService = new PokemonService(_httpClientFactory);

        //act
        Func<Task> action = async () => await _pokemonService.GetAsync(null!);

        //assert
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task GetAsync_Returns_Pokemon()
    {
        //arrange
        var mewtwo =
            await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Testdata", "mewtwo.json"));
        _httpClientFactory.CreateClient(Arg.Is(nameof(PokemonServiceOptions.PokemonApi)))
            .Returns(new HttpClient(new MockHttpStatusCodeHandler(HttpStatusCode.OK, new StringContent(mewtwo)))
                {BaseAddress = new Uri("http://localhost")});

        _pokemonService = new PokemonService(_httpClientFactory);

        //act
        var result = await _pokemonService.GetAsync("mewtwo");

        //assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Success.Should().BeTrue();
        result.Content.Should().NotBeNull();
        result.Content!.Name.Should().Be("mewtwo");
        result.Content!.Description.Should()
            .Be(
                "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.");
        result.Content.Habitat.Should().Be("rare");
        result.Content.IsLegendary.Should().BeTrue();
    }
}