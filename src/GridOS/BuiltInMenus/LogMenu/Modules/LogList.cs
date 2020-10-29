using System;

namespace IngameScript
{
    /// <summary>
    /// Populates a menu with the stream of incoming logging events of the type specified in the constructor.
    /// </summary>
    class LogList : MenuGroup
    {
        private readonly IMenuCommand _clearCommand;
        private readonly ILoggingEvents _eventSource;
        private readonly string _listingName;

        public LogList(ILoggingEvents eventSource, LogLevel logType) : base("")
        {
            _eventSource = eventSource;
            _clearCommand = new MenuCommand("Clear list", ResetList);

            switch (logType)
            {
                case LogLevel.Debug:
                    throw new Exception("Debug log listing currently not supported.");
                case LogLevel.Information:
                    _listingName = "Info logged";
                    eventSource.InformationLogged += OnMessage;
                    break;
                case LogLevel.Warning:
                    _listingName = "Warnings logged";
                    eventSource.WarningLogged += OnMessage;
                    break;
                case LogLevel.Error:
                    _listingName = "Errors logged";
                    eventSource.ErrorLogged += OnMessage;
                    break;
                default:
                    throw new Exception($"Unexpected value '{logType}' encountered.");
            }

            ResetList();
        }

        private void ResetList()
        {
            _children.Clear();
            AddChild(_clearCommand);
            SetLabel();
        }

        private void OnMessage(string message)
        {
            var logMessage = new MenuGroup(DateTime.Now.ToString());
            logMessage.AddChild(new MenuItem(message));
            AddChild(logMessage);
            SetLabel();
        }

        private void SetLabel()
        {
            Label = $"{_listingName}: {_children.Count - 1}"; // -1 for Clear List command.
        }
    }
}
