using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace PokeTranslator.Tests.Unit.Helpers;

public class MockHttpStatusCodeHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _status;
    private readonly HttpContent? _content;

    public MockHttpStatusCodeHandler(HttpStatusCode status, HttpContent? content = null)
    {
        _status = status;
        _content = content;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _status,
            Content = _content
        });
}