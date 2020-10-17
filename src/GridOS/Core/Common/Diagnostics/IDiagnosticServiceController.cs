namespace IngameScript
{
    public interface IDiagnosticServiceController : IDiagnosticService
    {
        LogLevel LoggingLevel { get; set; }
        void RecordExecution(bool logExecutionStats);
    }
}
