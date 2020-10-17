namespace IngameScript
{
    class LogLevelCommand : MenuCommand
    {
        private readonly IDiagnosticServiceController _diagnostics;

        public LogLevelCommand(IDiagnosticServiceController diagnostics) : base("", null)
        {
            _diagnostics = diagnostics;

            SetCommandLabel();
            _command = SwitchLoggingLevel;
        }

        private void SetCommandLabel()
        {
            Label = $"Logging level: {_diagnostics.LoggingLevel}";
        }

        private void SwitchLoggingLevel()
        {
            LogLevel newLogLevel;
            switch (_diagnostics.LoggingLevel)
            {
                case LogLevel.Debug:
                    newLogLevel = LogLevel.Information;
                    break;
                case LogLevel.Information:
                    newLogLevel = LogLevel.Warning;
                    break;
                case LogLevel.Warning:
                    newLogLevel = LogLevel.Error;
                    break;
                case LogLevel.Error:
                default:
                    newLogLevel = LogLevel.Debug;
                    break;
            }

            _diagnostics.LoggingLevel = newLogLevel;
            SetCommandLabel();
        }
    }
}
