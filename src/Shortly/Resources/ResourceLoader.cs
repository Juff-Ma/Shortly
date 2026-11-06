using Shortly.Setup;

namespace Shortly.Resources;

public static class ResourceLoader
{
    public static Stream GetDefaultConfigStream()
    {
        const string resourceName = $"{CommonConstants.AppName}.Resources.Config.Default.json";
        return CommonConstants.MainAssembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");
    }
}
