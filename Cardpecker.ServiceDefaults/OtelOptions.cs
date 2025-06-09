namespace Cardpecker.ServiceDefaults;

public class OtelOptions
{
    public const string Section = "OpenTelemetry";
    
    public required Uri CollectorUri { get; set; }
    public required string ServiceName { get; set; }
    public required string InstanceName { get; set; }
}