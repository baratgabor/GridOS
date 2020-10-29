using System;

namespace IngameScript
{
    public interface IGlobalEvents
    {
        event Action ExecutionJustStarted;
        event Action ExecutionWillFinish;
    }
}
