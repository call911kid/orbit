namespace Orbit.PL.Configuration;

public sealed class HangfireOptions
{
    public const string SectionName = "Hangfire";
    public bool RescheduleOnStartup { get; set; }
}
