using System.Diagnostics;

namespace Cardpecker.Api.Otel;

public class CardpeckerTracing
{
    public static ActivitySource Source { get; } = new ActivitySource("cardpecker.api");
}
