using System.Diagnostics.CodeAnalysis;

namespace Api.Configuration;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated by dependency injection.")]
internal sealed record ScrapeSettings
{
    public string EventsUrl { get; init; } = string.Empty;
}
