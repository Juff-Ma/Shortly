using System.Diagnostics.CodeAnalysis;

namespace Shortly.Helpers.Extensions;

internal static class HttpContextExtensions
{
    public static bool TryGetMetadata<T>(this HttpContext context,
        [NotNullWhen(true)] out T? metadata) 
        where T : class
    {
        metadata = context.GetEndpoint()?.Metadata.GetMetadata<T>();
        return metadata is not null;
    }
}