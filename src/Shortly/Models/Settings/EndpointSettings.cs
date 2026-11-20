namespace Shortly.Models.Settings;

public sealed class EndpointSettings
{
    // By default, accept all domains
    public string Host { get; set; } = "*";
    public string? Path { get; set; }
}


