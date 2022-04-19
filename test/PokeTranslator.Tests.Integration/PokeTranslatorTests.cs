using System.IO;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VerifyTests;
using VerifyXunit;
using Xunit;

namespace PokeTranslator.Tests.Integration;

[UsesVerify]
public class PokeTranslatorTests : IClassFixture<PokeApplication>
{
    protected readonly PokeApplication _application;
    private static readonly VerifySettings _verifierSettings = new();

    public PokeTranslatorTests(PokeApplication application)
    {
        _application = application;
        _verifierSettings.UseDirectory("Snapshots");
        _verifierSettings.ScrubInlineGuids();
    }

    [Fact]
    public async Task GetAsync_Returns_Pokemon_When_Pokemon_Found()
    {
        //arrange
        var client = _application.CreateClient();

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
        var client = _application.CreateClient();

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
        var client = _application.CreateClient();

        //act
        var response = await client.GetAsync("pokemon/translated/mewtwo");
        var content = await response.Content.ReadAsStringAsync();

        //assert
        await Verifier.Verify(new {message = response, content}, _verifierSettings);
    }
}


public class PokeApplication : WebApplicationFactory<Program>
{
    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        builder
            .UseConfiguration(configuration)
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostContext, config) =>
                config.AddJsonFile("appsettings.test.json", optional: false).AddEnvironmentVariables())
            .ConfigureServices(
                (context, services) => { });


        return base.CreateServer(builder);
    }

    // protected override void ConfigureWebHost(IWebHostBuilder builder)
    // {
    //     IConfiguration configuration = new ConfigurationBuilder()
    //         .SetBasePath(Directory.GetCurrentDirectory())
    //         .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true)
    //         .AddEnvironmentVariables()
    //         .Build();
    //     
    //     builder.ConfigureServices(services =>
    //     {
    //         var pokemonOptions = new PokemonServiceOptions();
    //         configuration.GetSection(PokemonServiceOptions.Name).Bind(pokemonOptions);
    //         services.AddHttpClient(nameof(PokemonServiceOptions.PokemonApi))
    //             .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.PokemonApi))
    //             .ConfigurePrimaryHttpMessageHandler(() => new RecordingHandler());
    //         services.AddHttpClient(nameof(TranslationOptions.Yoda))
    //             .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Yoda))
    //             .ConfigurePrimaryHttpMessageHandler(() => new RecordingHandler());
    //         services.AddHttpClient(nameof(TranslationOptions.Shakespeare))
    //             .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Shakespeare))
    //             .ConfigurePrimaryHttpMessageHandler(() => new RecordingHandler());
    //
    //     });
    //     base.ConfigureWebHost(builder);
    // }

    protected override IHost CreateHost(IHostBuilder builder)
    {
    
    
        var test = builder is IWebHostBuilder;
        
        builder
            
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseEnvironment("test")
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.Sources.Clear();
                config.AddJsonFile("appsettings.test.json", optional: false);
                config.AddEnvironmentVariables();
            });
    
        
        return base.CreateHost(builder);
    }
}

