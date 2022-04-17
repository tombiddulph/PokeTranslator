
using PokeTranslator;
using PokeTranslator.Config;
using PokeTranslator.Helpers;
using PokeTranslator.Middleware;
using Serilog;

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
    

    builder.Services.AddHttpClient(nameof(pokemonOptions.PokemonApi))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.PokemonApi))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());
    builder.Services.AddHttpClient(nameof(TranslationOptions.Yoda))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Yoda))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());
    builder.Services.AddHttpClient(nameof(TranslationOptions.Shakespeare))
        .ConfigureHttpClient(x => x.BaseAddress = new Uri(pokemonOptions.Translations.Shakespeare))
        .AddPolicyHandler(PolicyHelper.GetRetryPolicy());

    builder.Services.AddTransient<IPokemonService, PokemonService>();

    Log.Logger =  new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateBootstrapLogger();
    builder.Host.UseSerilog((context, logConfig) =>
    {
        logConfig.ReadFrom.Configuration(builder.Configuration);
    });

    
    var app = builder.Build();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Logger.Information( builder.Configuration.GetDebugView());
    }

    Serilog.Debugging.SelfLog.Enable(Console.Error);
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ErrorMiddleware>();
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging(opts =>
    {
        opts.IncludeQueryInRequestPath = true;
        opts.EnrichDiagnosticContext = LogEnricher.EnrichFromRequest;
    });

    app.UseAuthorization();

    app.MapControllers();

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