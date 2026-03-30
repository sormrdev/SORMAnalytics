using System.Text.Json;

using Carter;

using Microsoft.Net.Http.Headers;

namespace Web.Infrastructure.Negotiators;
public class HateoasResponseNegotiator : IResponseNegotiator
{
    public bool CanHandle(MediaTypeHeaderValue accept)
    {
        var matches = accept.MatchesMediaType(CustomMediaTypeNames.Application.HateoasJson);
        Console.WriteLine($"CanHandle: {accept} matches? {matches}");
        return matches;
    }

    public async Task Handle<T>(HttpRequest req, HttpResponse res, T model, CancellationToken cancellationToken)
    {
        res.ContentType = CustomMediaTypeNames.Application.HateoasJson;
        await JsonSerializer.SerializeAsync(res.Body, model, cancellationToken: cancellationToken);
    }
}