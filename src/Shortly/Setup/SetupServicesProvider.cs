using JasperFx;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.Options;
using Shortly.Services.Logging;

namespace Shortly.Setup;

public static class SetupServicesProvider
{
    public static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        services.CritterStackDefaults(opts =>
        {
            opts.Production.GeneratedCodeMode = TypeLoadMode.Static;

            opts.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
        });

        services.AddRoutingCore();

        services.AddSingleton<LogMonitorService>();

        services.AddShortlyLogging(context.Configuration);
    }
}
