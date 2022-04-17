using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using PokeTranslator.Config;
using PokeTranslator.Helpers;
using PokeTranslator.Middleware;
using PokeTranslator.Model;
using PokeTranslator.Services;
using Prometheus;
using Serilog;
using static PokeTranslator.Helpers.Extensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var pokemonOptions = new PokemonServiceOptions();
    builder.Configuration.GetSection(PokemonServiceOptions.Name).Bind(pokemonOptions);


    builder.Services.AddHttpClient(nameof(PokemonServiceOptions.PokemonApi))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.PokemonApi))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());
    builder.Services.AddHttpClient(nameof(TranslationOptions.Yoda))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Yoda))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());
    builder.Services.AddHttpClient(nameof(TranslationOptions.Shakespeare))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Shakespeare))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());

    builder.Services.AddTransient<IPokemonService, PokemonService>();

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateBootstrapLogger();
    builder.Host.UseSerilog((context, logConfig) => { logConfig.ReadFrom.Configuration(builder.Configuration); });

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
    //app.UseEndpoints(endpoints => endpoints.MapMetrics());
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

            return await pokemonService.GetAsync(name)! switch
            {
                { } pokemon => Results.Ok(pokemon),
                _ => Results.NotFound(new ProblemDetails
                {
                    Detail = $"Pokemon '{name}' not found", Status = 404, Title = "Not Found",
                })
            };
        });

    app.MapGet("/pokemon/translated/{name}",
        async (string name, IPokemonService pokemonService) =>
        {
            HttpResult<PokemonResponse> a = (await pokemonService.TranslateAsync(name));
            return GetResult(a, name);
        });
    app.Run();

   
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