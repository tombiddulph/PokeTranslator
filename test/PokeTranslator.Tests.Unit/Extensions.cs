using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace PokeTranslator.Tests.Unit;

public static class Extensions
{
    public static Dictionary<string, string> ToDictionary(this HttpHeaders headers) =>
        headers
            .ToDictionary(x => x.Key, x => string.Join("|", x.Value));

}