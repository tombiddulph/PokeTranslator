using Polly;

namespace PokeTranslator.Helpers;

public class PolicyHelper
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() =>
        Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}