using System;

namespace IngameScript
{
    public class GlobalEventDispatcher : IGlobalEvents, ILoggingEvents
    {
        public event Action ExecutionJustStarted;
        public event Action ExecutionWillFinish;

        public event Action<string> ErrorLogged;
        public event Action<string> WarningLogged;
        public event Action<string> InformationLogged;
        public event Action<string> DebugLogged;

        public void OnExecutionStarted()
            => ExecutionJustStarted?.Invoke();

        public void OnExecutionFinishing()
            => ExecutionWillFinish?.Invoke();
        

        public void OnErrorLogged(string message)
            => ErrorLogged?.Invoke(message);

        public void OnWarningLogged(string message)
            => WarningLogged?.Invoke(message);

        public void OnInformationLogged(string message)
            => InformationLogged?.Invoke(message);

        public void OnDebugLogged(string message)
            => DebugLogged?.Invoke(message);
    }
}
