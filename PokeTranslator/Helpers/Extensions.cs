using Microsoft.AspNetCore.Mvc;
using System.Net;
using PokeTranslator.Model;

namespace PokeTranslator.Helpers;

public static class Extensions
{
    public static IResult TooManyRequestsResult(this IResultExtensions extensions)
    {
        return Results.Problem(statusCode: 429, title: "Too many requests",
            detail: "Too many requests to translation api. Please try again later.");
    }

    public static IResult GetResult(HttpResult<PokemonResponse> httpResult, string name) =>
        httpResult switch
        {
            {Success: true} r => Results.Json(r.Content),
            {StatusCode: HttpStatusCode.NotFound} => Results.NotFound(new ProblemDetails
            {
                Detail = $"Pokemon '{name}' not found", Status = 404, Title = "Not Found",
            }),
            {StatusCode: HttpStatusCode.TooManyRequests} => Results.Extensions.TooManyRequestsResult(),
            _ => Results.Problem(statusCode: (int) httpResult.StatusCode)
        };
}