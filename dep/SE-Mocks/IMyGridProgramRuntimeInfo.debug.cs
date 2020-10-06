using System;

namespace IngameScript
{
    public interface IMyGridProgramRuntimeInfo
    {
        TimeSpan TimeSinceLastRun { get; set; }

        double LastRunTimeMs { get; set; }

        int MaxInstructionCount { get; set; }

        int CurrentInstructionCount { get; set; }

        int MaxCallChainDepth { get; set; }

        int CurrentCallChainDepth { get; set; }

        UpdateFrequency UpdateFrequency { get; set; }
    }
}
