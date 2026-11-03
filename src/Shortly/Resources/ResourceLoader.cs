using System.Reflection;

namespace Shortly.Resources;

public static class ResourceLoader
{
    private const string AppName = "Shortly";

    public static readonly Assembly ThisAssembly 
        = typeof(ResourceLoader).Assembly;

    public static Stream GetDefaultConfigStream()
    {
        const string resourceName = $"{AppName}.Resources.Config.Default.json";
        return ThisAssembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");
    }
}
