namespace Shortly.Setup;

public static class SetupEndpointsExtensions
{
    public static IApplicationBuilder UseShortlyEndpoints(this IApplicationBuilder builder,
        WebHostBuilderContext context)
        => builder.UseEndpoints((app) =>
        {

        });
}
