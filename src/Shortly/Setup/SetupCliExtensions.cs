using JasperFx;
using System.Diagnostics.CodeAnalysis;

namespace Shortly.Setup;

public static class SetupCliExtensions
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "JasperFx.SourceGenerator registers the commands.")]
    public static IHostBuilder ApplyShortlyCliExtensions(this IHostBuilder builder)
        => builder.ApplyJasperFxExtensions();

    [UnconditionalSuppressMessage("Trimming", "IL2026:RequiresUnreferencedCode",
        Justification = "JasperFx.SourceGenerator registers the commands")]
    [UnconditionalSuppressMessage("AOT", "IL3050:RequiresDynamicCode",
        Justification = "Every command input has a source-generated parser, so no List<T> is closed at runtime.")]
    public static Task<int> BuildAndRunWithCli(this IHostBuilder builder, string[] args)
        => builder.RunJasperFxCommands(args);
}
