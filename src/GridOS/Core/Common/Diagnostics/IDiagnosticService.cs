using System;

namespace IngameScript
{
    public interface IDiagnosticService : ILogger
    {
        double LastRunTimeMs { get; }
        double AverageRunTimeMs { get; }
        TimeSpan TimeSinceLastRun { get; }
        long NumberOfRuns { get; }
    }
}
