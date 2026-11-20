using Shortly.Helpers.Extensions;
using Shortly.Models.Settings;

namespace Shortly.Setup;

public static class SetupEndpointsExtensions
{
    public static IApplicationBuilder UseShortlyEndpoints(this IApplicationBuilder builder,
        WebHostBuilderContext context)
        => builder.UseEndpoints((app) =>
        {
            var endpoints = context.Configuration
                .GetSection("Urls")
                .Get<List<EndpointSettings>>();

            if (endpoints is not { Count: > 0 })
            {
                // There is a fallback in the default configuration,
                // but if the user has overridden it and removed all endpoints, we should throw an exception.
                throw new InvalidOperationException("No endpoints configured in the 'Urls' section.");
            }

            foreach (var endpoint in endpoints)
            {
                app.MapGroup($"{endpoint.Path}")
                    .RequireHost(endpoint.Host)
                    .WithMetadata(endpoint)
                    .MapShortlyGroup(context);
            }
        });

    private static RouteGroupBuilder MapShortlyGroup(this RouteGroupBuilder group, WebHostBuilderContext context)
    {
        group.MapGet("/", IResult (HttpContext ctx) =>
        {
            if (!ctx.TryGetMetadata<EndpointSettings>(out var meta))
            {
                return Results.InternalServerError();
            }

            return Results.Ok($"Host: {meta.Host}, Path: {meta.Path}");
        });

        return group;
    }
}
