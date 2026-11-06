using System.Reflection;

namespace Shortly.Setup;

public static class CommonConstants
{
    public const string AppName = "Shortly";

    public static readonly Assembly MainAssembly
        = typeof(CommonConstants).Assembly;

    public const string ConsoleLogTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] - {Message:lj}{NewLine}{Exception}";
    public const string FileLogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level}] {SourceContext} : {Message:lj}{NewLine}{Exception}";
}
