using System.Text;

namespace IngameScript
{
    public interface ILogger
    {
        /// <summary>
        /// Logs the message. Use the other overloads to avoid string allocation in case the logging threshold is above the loglevel of the message.
        /// </summary>
        void Log(LogLevel logLevel, string message);
        void Log(LogLevel logLevel, string formatString, object param1, object param2 = null, object param3 = null);
        void Log(LogLevel logLevel, StringBuilder message);
        void Log(LogLevel logLevel, StringSegment message);
    }
}
