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
            opts.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            opts.Development.ResourceAutoCreate = AutoCreate.CreateOrUpdate;

            opts.Production.GeneratedCodeMode = TypeLoadMode.Static;
            opts.Production.ResourceAutoCreate = AutoCreate.None;
            opts.Production.AssertAllPreGeneratedTypesExist = true;

            opts.ApplicationAssembly = CommonConstants.MainAssembly;
        });

        services.AddRoutingCore();

        services.AddSingleton<LogMonitorService>();

        services.AddShortlyLogging(context.Configuration);
    }
}
