using System.Diagnostics.CodeAnalysis;

namespace Api.CronJobs;

/// <summary>
/// A background service that runs daily tasks at 3 AM Eastern Time.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "This class is instantiated by dependency injection.")]
internal sealed class Daily(ILogger<Daily> logger) : BackgroundService
{
    private static readonly TimeZoneInfo EasternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    private static readonly Action<ILogger, DateTimeOffset, Exception?> LogDailyJobRunning =
        LoggerMessage.Define<DateTimeOffset>(
            LogLevel.Information,
            new EventId(1, nameof(LogDailyJobRunning)),
            "Daily job running at: {Time}");

    private readonly int scheduledHour = 21;
    private readonly int scheduledMinute = 42; // = 0;

    private readonly ILogger<Daily> logger = logger;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, EasternTimeZone);
            var nextRun = now.Date
                .AddHours(scheduledHour)
                .AddMinutes(scheduledMinute);

            if (now >= nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var delay = nextRun - now;

            // Wait until the scheduled time
            await Task.Delay(delay, stoppingToken).ConfigureAwait(false);

            // Execute the job
            LogDailyJobRunning(logger, now, null);
        }
    }
}
