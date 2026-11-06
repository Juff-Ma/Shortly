using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Shortly.Services.Logging;

namespace Shortly.Setup;

public static class SetupLoggingExtensions
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

    public static IHostBuilder ApplyHostLogging(this IHostBuilder builder)
    {
        return builder.ConfigureLogging((context, config) =>
        {
            config.ClearProviders();
            config.AddConfiguration(context.Configuration.GetSection("Logging"));

            // By setting the minimum level to None, we can later detect if it has been changed by command line
            // If it has been, we ignore the logging configuration in the config file and use the command line configuration instead
            // See LogMonitorService for more details
            // NOTE: this doesn't mean logs can't be disabled
            //       but this isn't something you should do over the debug cmd switch
            config.SetMinimumLevel(LogLevel.None);
        });
    }

    public static LoggerConfiguration ReadFromLogMonitor(this LoggerConfiguration config, IServiceProvider services)
    {
        var monitor = services.GetRequiredService<LogMonitorService>();

        config.MinimumLevel.ControlledBy(monitor.DefaultLevelSwitch);

        foreach (var (source, @switch) in monitor.OverrideSwitches)
        {
            config.MinimumLevel.Override(source, @switch);
        }
        
        return config;
    }

    public static IServiceCollection AddShortlyLogging(this IServiceCollection services,
        IConfiguration config,
        string consoleLogTemplate = CommonConstants.ConsoleLogTemplate,
        string fileLogTemplate = CommonConstants.FileLogTemplate)
    {
        bool logToConsole = config.GetValue("Logging:Console", defaultValue: true);
        bool logToFile = config.GetValue("Logging:File", defaultValue: false);

        string logFileDir = config.GetValue<string>("Logging:LogFileDirectory", defaultValue: "logs/");

        return services.AddSerilog((ls, lc) =>
        {
            lc.WithCommonConfiguration()
                .ReadFromLogMonitor(ls);

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
}
