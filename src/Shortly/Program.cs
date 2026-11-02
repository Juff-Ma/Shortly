using Serilog;
using System.Reflection;
using Tomlyn.Extensions.Configuration;

Log.Logger = new LoggerConfiguration()
    .WithCommonConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateSlimBuilder();

    builder.Host
        .AddShortlyConfiguration(args);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {

    }

    await app.RunAsync();
}
catch (Exception e)
{
    Log.Fatal(e, "An unrecoverable error occured during application startup.");
#if DEBUG
    throw;
#endif
}
finally
{
    await Log.CloseAndFlushAsync();
}

static class CommonExtensions
{
    public static LoggerConfiguration WithCommonConfiguration(this LoggerConfiguration config)
    {
        return config
#if DEBUG
            .WriteTo.Debug()
#endif
            .Enrich.FromLogContext();
    }

    public static IHostBuilder AddShortlyConfiguration(this IHostBuilder builder, string[] args)
    {
        return builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();

            var env = context.HostingEnvironment;
            bool reloadOnChange = context.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);

            config.AddTomlFile("shortly.toml", optional: true, reloadOnChange: reloadOnChange)
                .AddTomlFile("Shortly.toml", optional: true, reloadOnChange: reloadOnChange)
                .AddTomlFile($"shortly.{env.EnvironmentName.ToLowerInvariant()}.toml", optional: true, reloadOnChange: reloadOnChange)
                .AddTomlFile($"Shortly.{env.EnvironmentName}.toml", optional: true, reloadOnChange: reloadOnChange);

#if DEBUG
            if (env.IsDevelopment() && env.ApplicationName is { Length: > 0 })
            {
                var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                config.AddUserSecrets(appAssembly, optional: true, reloadOnChange: reloadOnChange);
            }
#endif
            config.AddEnvironmentVariables();

            // Only allow long form arguments
            var filteredArgs = args.Where(arg => arg.Contains('=')).ToArray();
            if (filteredArgs.Length > 0)
            {
                config.AddCommandLine(filteredArgs);
            }
        });
    }
}