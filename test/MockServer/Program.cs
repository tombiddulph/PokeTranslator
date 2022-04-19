using System.Text.Json;
using Microsoft.AspNetCore.HttpLogging;
using MockServer.Models;
using O9d.Json.Formatting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, configuration) => configuration.WriteTo.Console().MinimumLevel.Debug());
var app = builder.Build();
app.UseSerilogRequestLogging();

var testFiles = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "TestData")).ToHashSet();

var serializerOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
};

app.MapGet("/pokemon/{name}", (string name) =>
{
    //var name = context.Request.RouteValues["name"]?.ToString();
    if (string.IsNullOrEmpty(name))
    {
        return Results.BadRequest();
    }

    if (testFiles.FirstOrDefault(x => x.Contains(name)) is { } filePath)
    {
        var file = File.ReadAllText(filePath);
        
        return Results.Json(JsonSerializer.Deserialize<PokemonSpecies>(file, serializerOptions), serializerOptions, contentType: "application/json");
    }

    return Results.NotFound();
});

app.MapPost("/translate/yoda.json", (TranslateRequest request) =>
{
    if (request == null || string.IsNullOrEmpty(request.text))
    {
        return Results.BadRequest();
    }

    if (string.Equals(request.text,
            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
            StringComparison.CurrentCultureIgnoreCase))
    {
        var file = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestData", "mew-translated.json"));
        return Results.Json(JsonSerializer.Deserialize<TranslateResponse>(file), contentType: "application/json");
    }

    return Results.NotFound();
});

app.MapPost("/translate/shakespeare.json", (TranslateRequest request) =>
{
    if (request == null || string.IsNullOrEmpty(request.text))
    {
        return Results.BadRequest();
    }

    if (string.Equals(request.text,
            "It was created by\na scientist after\nyears of horrific\fgene splicing and\nDNA engineering\nexperiments.",
            StringComparison.CurrentCultureIgnoreCase))
    {
        var file = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestData", "mew-translated.json"));
        return Results.Json(JsonSerializer.Deserialize<TranslateResponse>(file), contentType: "application/json");
    }

    return Results.NotFound();
});


app.Run();


record TranslateRequest(string text);

record TranslateResponse(Success success, Contents contents);

record Success(int total);

record Contents(string translated, string text, string translation);