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
        private readonly GlobalEventDispatcher _eventDispatcher;

        public GridProgramDiagnostics(MyGridProgram program, GlobalEventDispatcher eventDispatcher)
        {
            _program = program;
            _eventDispatcher = eventDispatcher;
        }

        /// <summary>
        /// Use the overloads instead, because they won't allocate a string if the specified loglevel is below the logging level set.
        /// </summary>
        public void Log(LogLevel logLevel, string message)
        {
            if (logLevel < LoggingLevel)
                return;

            LogInternal(message, logLevel);
        }

        public void Log(LogLevel logLevel, StringBuilder message)
        {
            if (logLevel < LoggingLevel)
                return;

            LogInternal(message.ToString(), logLevel);
        }

        public void Log(LogLevel logLevel, StringSegment message)
        {
            if (logLevel < LoggingLevel)
                return;

            LogInternal(message.ToString(), logLevel);
        }

        public void RecordExecution(bool logExecutionStats)
        {
            NumberOfRuns++;
            AverageRunTimeMs += (LastRunTimeMs - AverageRunTimeMs) / Math.Min(NumberOfRuns, 5);

            if (logExecutionStats)
                LogExecutionStatistics();
        }

        public void Log(LogLevel logLevel, string formatString, object arg0, object arg1 = null, object arg2 = null)
        {
            if (logLevel < LoggingLevel)
                return;

            var formattedString = string.Format(formatString, arg0, arg1, arg2);

            LogInternal(formattedString, logLevel);
        }

        private void LogInternal(string message, LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Error:
                    _eventDispatcher.OnErrorLogged(message);
                    break;
                case LogLevel.Warning:
                    _eventDispatcher.OnWarningLogged(message);
                    break;
                case LogLevel.Information:
                    _eventDispatcher.OnInformationLogged(message);
                    break;
                case LogLevel.Debug:
                    _eventDispatcher.OnDebugLogged(message);
                    break;
                default:
                    break;
            }

            _program.Echo(message);
        }

        private void LogExecutionStatistics()
        {
            _program.Echo(string.Format("Execution no. {0}.\r\nAverage Execution Time: {1:G3}ms", NumberOfRuns, AverageRunTimeMs));
        }
    }
}
