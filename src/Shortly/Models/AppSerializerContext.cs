using Shortly.Models.Settings;
using System.Text.Json.Serialization;

namespace Shortly.Models;

/// <summary>
/// This is the default serializer for this app.
/// This is not required for 90% of types but some we need to register manually.
/// </summary>
[JsonSerializable(typeof(EndpointSettings))]
[JsonSerializable(typeof(List<EndpointSettings>))]
internal partial class AppSerializerContext : JsonSerializerContext;