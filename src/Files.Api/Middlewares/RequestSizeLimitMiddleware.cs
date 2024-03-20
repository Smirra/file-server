using Microsoft.AspNetCore.Http.Features;

namespace Files.Api.Middlewares;
public class RequestSizeLimitMiddleware
{
    readonly IConfiguration config;
    private readonly RequestDelegate _next;
    private readonly List<string> _restrictedEndpoints = [""];

    private readonly long BODY_SIZE_LIMIT;

    public RequestSizeLimitMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        config = configuration;
        // Adding 0.5 for overhead.
        BODY_SIZE_LIMIT = (int)(int.Parse(config["BODY_SIZE_LIMIT_MB"]
                                            ?? throw new Exception("BODY_SIZE_LIMIT_MB not set.")) + 0.5 * 1_048_576);
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (_restrictedEndpoints.Any(endpoint => context.Request.Path.StartsWithSegments(endpoint, StringComparison.OrdinalIgnoreCase)))
        {
            var httpMaxRequestBodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (httpMaxRequestBodySizeFeature != null)
            {
                httpMaxRequestBodySizeFeature.MaxRequestBodySize = BODY_SIZE_LIMIT;
            }
        }

        await _next(context);
    }
}
