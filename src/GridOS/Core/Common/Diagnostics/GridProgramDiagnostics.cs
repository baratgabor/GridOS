using System;
using System.Text;

namespace IngameScript
{
    class GridProgramDiagnostics : IDiagnosticServiceController
    {
        public LogLevel LoggingLevel { get; set; } = LogLevel.Error;
        public double LastRunTimeMs => _program.Runtime.LastRunTimeMs;
        public double AverageRunTimeMs { get; private set; }
        public TimeSpan TimeSinceLastRun => _program.Runtime.TimeSinceLastRun;
        public long NumberOfRuns { get; private set; }

        private readonly MyGridProgram _program;

        public GridProgramDiagnostics(MyGridProgram program)
        {
            _program = program;
        }

        /// <summary>
        /// Use the overloads instead, because they won't allocate a string if the specified loglevel is below the logging level set.
        /// </summary>
        public void Log(LogLevel logLevel, string message)
        {
            if (logLevel < LoggingLevel)
                return;

            _program.Echo(message);
        }

        public void Log(LogLevel logLevel, StringBuilder message)
        {
            if (logLevel < LoggingLevel)
                return;

            Log(message.ToString());
        }

        public void Log(LogLevel logLevel, StringSegment message)
        {
            if (logLevel < LoggingLevel)
                return;

            Log(message.ToString());
        }

        public void RecordExecution(bool logExecutionStats)
        {
            NumberOfRuns++;
            AverageRunTimeMs += (LastRunTimeMs - AverageRunTimeMs) / Math.Min(NumberOfRuns, 5);

            if (logExecutionStats)
                LogExecutionStatistics();
        }

        private void Log(string message)
        {
            _program.Echo(message);
        }

        private void LogExecutionStatistics()
        {
            if (LoggingLevel > LogLevel.Information)
                return;

            _program.Echo(string.Format("Execution no. {0}.\r\nAverage Execution Time: {1:G3}ms", NumberOfRuns, AverageRunTimeMs));
        }

        public void Log(LogLevel logLevel, string formatString, object arg0, object arg1 = null, object arg2 = null)
        {
            if (logLevel < LoggingLevel)
                return;

            var formattedString = string.Format(formatString, arg0, arg1, arg2);

            Log(formattedString);
        }
    }
}
