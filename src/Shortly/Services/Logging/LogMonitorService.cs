using Microsoft.Extensions.Options;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;

namespace Shortly.Services.Logging;

public class LogMonitorService
{
    public LogMonitorService(IOptionsMonitor<LoggerFilterOptions> monitor)
    {
        DefaultLevelSwitch = new LoggingLevelSwitch();
        OverrideSwitches = new Dictionary<string, LoggingLevelSwitch>();

        var current = monitor.CurrentValue;

        if (current.MinLevel == LogLevel.None)
        {
            Init(current.Rules);
        }

        UpdateLogLevels(current);
        monitor.OnChange(UpdateLogLevels);
    }

    private void Init(IList<LoggerFilterRule> rules)
    {
        foreach (var rule in rules
                     .Where(r =>
                         r.CategoryName is not null))
        {
            LoggingLevelSwitch @switch = new()
            {
                MinimumLevel = LevelAlias.Off
            };

            OverrideSwitches[rule.CategoryName!] = @switch;
        }
    }

    void UpdateLogLevels(LoggerFilterOptions options)
    {
        if (options.MinLevel != LogLevel.None)
        {
            DefaultLevelSwitch.MinimumLevel = LevelConvert.ToSerilogLevel(options.MinLevel);
            return;
        }

        DefaultLevelSwitch.MinimumLevel = LevelConvert.ToSerilogLevel(
            options.Rules
                .Single(r 
                    => r.CategoryName is null)
                .LogLevel ?? LogLevel.Information
            );

        foreach (var rule in options.Rules
                     .Where(r => 
                         r.CategoryName is not null && 
                         r.LogLevel is not null))
        {
            var @switch = OverrideSwitches[rule.CategoryName!];

            @switch.MinimumLevel = LevelConvert.ToSerilogLevel(rule.LogLevel!.Value);
        }
    }

    public LoggingLevelSwitch DefaultLevelSwitch { get; }

    public Dictionary<string, LoggingLevelSwitch> OverrideSwitches { get; }
}
