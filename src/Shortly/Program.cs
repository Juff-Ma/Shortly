using Serilog;
using Serilog.Events;
using Serilog.Settings.Configuration;
using Shortly.Resources;
using Tomlyn.Extensions.Configuration;

Log.Logger = new LoggerConfiguration()
    .WithCommonConfiguration()
#if !DEBUG
    .MinimumLevel.Error()
#endif
    .WriteTo.Console(outputTemplate: CommonExtensions.ConsoleLogTemplate)
    .CreateBootstrapLogger();

var logger = Log.ForContext<Program>();

try
{
    var builder = WebApplication.CreateSlimBuilder();

    builder.Host
        .AddShortlyConfiguration(args);

    builder.Services.AddShortlyLogging(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {

    }

    await app.RunAsync();
}
catch (Exception e)
{
    logger.Fatal(e, "An unrecoverable error occured during application startup.");
#if DEBUG
    throw;
#endif
}
finally
{
    logger.Verbose("App has shut down.");
    await Log.CloseAndFlushAsync();
}

static class CommonExtensions
{
    public static LoggerConfiguration WithCommonConfiguration(this LoggerConfiguration config)
    {
        return config
#if DEBUG
            .WriteTo.Debug()
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Debug()
#endif
            .Enrich.FromLogContext();
    }

    public const string ConsoleLogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] - {Message:lj}{NewLine}{Exception}";
    public const string FileLogTemplate =    "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level}] {SourceContext} : {Message:lj}{NewLine}{Exception}";

    public static IServiceCollection AddShortlyLogging(this IServiceCollection services, 
        IConfiguration config,
        string consoleLogTemplate = ConsoleLogTemplate,
        string fileLogTemplate = FileLogTemplate)
    {
        bool logToConsole = config.GetValue("Logging:Console", defaultValue: true);
        bool logToFile = config.GetValue("Logging:File", defaultValue: false);

        string logFileDir = config.GetValue<string>("Logging:LogFileDirectory", defaultValue: "logs/");

        return services.AddSerilog((ls, lc) =>
        {
            lc.WithCommonConfiguration()
                .ReadFrom.Configuration(config, new ConfigurationReaderOptions
                {
                    SectionName = "Logging"
                })
                .ReadFrom.Services(ls);

            if (logToConsole)
            {
                lc.WriteTo.Console(restrictedToMinimumLevel:
#if DEBUG
                    LogEventLevel.Debug,
#else
                    LogEventLevel.Information,
#endif
                    outputTemplate: consoleLogTemplate);
            }

            if (logToFile)
            {
                lc.WriteTo.File($"{logFileDir}/shortly-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: fileLogTemplate);
            }

        });
    }

    public static IHostBuilder AddShortlyConfiguration(this IHostBuilder builder, string[] args)
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
                config.AddUserSecrets(ResourceLoader.ThisAssembly, optional: true, reloadOnChange: reloadOnChange);
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