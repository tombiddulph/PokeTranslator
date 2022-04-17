using System.Net;
using Polly;

namespace PokeTranslator.Helpers;

public class PolicyHelper
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode && r.StatusCode != HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(2, retryAttempt)));
}