using Prometheus;

namespace Infrastructure.Metrics;

public static class LogMetrics
{
    public static readonly Counter ErrorsLoggedTotal = Prometheus.Metrics
        .CreateCounter("errors_logged_total", "Total number of error log entries");

    public static readonly Counter LogsProcessedTotal = Prometheus.Metrics
        .CreateCounter("logs_processed_total", "Total number of log entries processed",
            new CounterConfiguration
            {
                LabelNames = new[] { "service", "level" }
            });

    public static readonly Histogram LogProcessingDuration = Prometheus.Metrics
        .CreateHistogram("log_processing_duration_seconds", "Time to process a log entry",
            new HistogramConfiguration
            {
                Buckets = Histogram.ExponentialBuckets(0.001, 2, 10)
            });
}