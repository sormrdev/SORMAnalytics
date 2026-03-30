using Microsoft.Net.Http.Headers;

namespace Web.Infrastructure;

public static class CustomMediaTypeNames
{
    public static class Application
    {
        public const string HateoasJson = "application/vnd.sormanalytics.hateoas";
        public static bool isHateoasJson(string? accept)
        {
            if (accept != null)
            {
                var mediaType = new MediaTypeHeaderValue(accept);
                return mediaType.MatchesMediaType(HateoasJson);
            }
            return false;
        }
    }
}
