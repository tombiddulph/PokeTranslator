using Serilog;

namespace PokeTranslator.Helpers;

//taken from https://benfoster.io/blog/serilog-best-practices/#request-log-enricher

public static class LogEnricher
{
    /// <summary>
    /// Enriches the HTTP request log with additional data via the Diagnostic Context
    /// </summary>
    /// <param name="diagnosticContext">The Serilog diagnostic context</param>
    /// <param name="httpContext">The current HTTP Context</param>
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        diagnosticContext.SetIfNotNull("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString());
        diagnosticContext.SetIfNotNull("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
        diagnosticContext.SetIfNotNull("CorrelationId", httpContext.Response.Headers["X-Correlation-ID"].FirstOrDefault());
    }
    
    public static void SetIfNotNull(this IDiagnosticContext context, string key, object? value)
    {
        if (value != null)
        {
            context.Set(key, value);
        }
    }
}