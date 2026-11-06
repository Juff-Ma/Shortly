using Shortly.Resources;
using Tomlyn.Extensions.Configuration;

namespace Shortly.Setup;

public static class SetupConfigurationExtensions
{
    private static readonly IReadOnlyDictionary<string, string?> DefaultHostConfiguration = 
        new Dictionary<string, string?>
    {
        { HostDefaults.EnvironmentKey, Environments.Production },
        { HostDefaults.ContentRootKey, AppContext.BaseDirectory },
        { HostDefaults.ApplicationKey, CommonConstants.AppName }
    };

    public static IHostBuilder ApplyHostConfiguration(this IHostBuilder builder)
    {
        return builder.ConfigureHostConfiguration((config) =>
        {
            config.Sources.Clear();
            config.SetBasePath(AppContext.BaseDirectory);

            config.AddInMemoryCollection(DefaultHostConfiguration);

            config.AddEnvironmentVariables(prefix: "DOTNET_");
            config.AddEnvironmentVariables(prefix: "ASPNETCORE_");

            config.AddIniFile("config.ini", optional: true, reloadOnChange: false);
        });
    }

    public static IHostBuilder ApplyShortlyConfiguration(this IHostBuilder builder)
    {
        return builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();

            config.SetBasePath(AppContext.BaseDirectory);

            config.AddJsonStream(ResourceLoader.GetDefaultConfigStream());

            var env = context.HostingEnvironment;
            bool reloadOnChange = context.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

            config.AddTomlFile("shortly.toml", optional: true, reloadOnChange: reloadOnChange)
                .AddTomlFile("Shortly.toml", optional: true, reloadOnChange: reloadOnChange)
                .AddTomlFile($"shortly.{env.EnvironmentName.ToLowerInvariant()}.toml", optional: true, reloadOnChange: reloadOnChange)
                .AddTomlFile($"Shortly.{env.EnvironmentName}.toml", optional: true, reloadOnChange: reloadOnChange);

#if DEBUG
            if (env.IsDevelopment())
            {
                config.AddUserSecrets(CommonConstants.MainAssembly, optional: true, reloadOnChange: reloadOnChange);
            }
#endif
            config.AddEnvironmentVariables(prefix: "SHORTLY_");
        });
    }
}
