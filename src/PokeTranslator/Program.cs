using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PokeTranslator.Config;
using PokeTranslator.Helpers;
using PokeTranslator.Middleware;
using PokeTranslator.Services;
using Prometheus;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
    builder.Configuration.AddEnvironmentVariables();

    var pokemonOptions = new PokemonServiceOptions();
    builder.Configuration.GetSection(PokemonServiceOptions.Name).Bind(pokemonOptions);
    builder.Services.AddSingleton(pokemonOptions);


    builder.Services.AddHttpClient(nameof(PokemonServiceOptions.PokemonApi))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.PokemonApi))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());
    builder.Services.AddHttpClient(nameof(TranslationOptions.Yoda))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Yoda))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());
    builder.Services.AddHttpClient(nameof(TranslationOptions.Shakespeare))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Shakespeare))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());

    builder.Services.AddMemoryCache();
    builder.Services.AddTransient<IPokemonService, PokemonService>();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateBootstrapLogger();
    
    
    Log.Logger.Information("Starting up");
    builder.Host.UseSerilog((context, logConfig) => logConfig.ReadFrom.Configuration(builder.Configuration));

    builder.Services.AddHealthChecks();


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Logger.Information(builder.Configuration.GetDebugView());
    }

    app.MapHealthChecks("/health");
    app.UseAuthorization();
    app.UseHttpMetrics();
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ErrorMiddleware>();
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(opts =>
    {
        opts.IncludeQueryInRequestPath = true;
        opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
    });
    
    app.MapControllers();


    app.MapGet("/pokemon/{name}",
        async ([Required] string name, [FromServices] IPokemonService pokemonService) =>
        {
            if (name.ToLower() == "translated")
            {
                return Results.BadRequest(new ProblemDetails
                {
                    Title = "Missing parameter",
                    Detail = "The name parameter is required"
                });
            }

            return (await pokemonService.GetAsync(name)).GetResult(name);
        });

    app.MapGet("/pokemon/translated/{name}",
        async (string name, IPokemonService pokemonService) =>
            (await pokemonService.TranslateAsync(name)).GetResult(name));
    app.Run();
    
    Log.Logger.Information("Shutting down");
}
catch (Exception ex)
{
    Log.Fatal(ex, "A startup exception occurred.");
}
finally
{
    Log.Information("Shutdown complete");
    Log.CloseAndFlush();
}

// Make the implicit Program class public so test projects can access it
public partial class Program { }