using System;

namespace IngameScript
{
    public class GlobalEventDispatcher : IGlobalEvents
    {
        public event Action ExecutionJustStarted;
        public event Action ExecutionWillFinish;

        public void ExecutionStarted()
            => ExecutionJustStarted?.Invoke();

        public void ExecutionFinishing()
            => ExecutionWillFinish?.Invoke();
    }
}
