using System.Text.Json;
using O9d.Json.Formatting;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


var testFiles = Directory.EnumerateFiles("TestData").ToHashSet();

app.MapGet("/", () => Results.NotFound());

app.MapGet("/pokemon/{name}", (string name) =>
{
    //var name = context.Request.RouteValues["name"]?.ToString();
    if (string.IsNullOrEmpty(name))
    {
        return Results.BadRequest();
    }

    if (testFiles.Contains(name))
    {
        var file = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "TestData", name));

        return Results.Json(file, new JsonSerializerOptions
        {
            PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
        });

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
        return Results.Json(JsonSerializer.Deserialize<TranslateResponse>(file), contentType:"application/json");
    }

    return Results.Ok();
});

app.Run();


record TranslateRequest(string text);

record TranslateResponse(Success success, Contents contents);
record Success(int total);
record Contents(string translated, string text, string translation);