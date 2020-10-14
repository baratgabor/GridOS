using System;

namespace IngameScript
{
    class FakeRuntimeInfo : IMyGridProgramRuntimeInfo
    {
        public TimeSpan TimeSinceLastRun { get; set; } = TimeSpan.Zero;

        public double LastRunTimeMs { get; set; } = 0.1d;
        public int MaxInstructionCount { get; set; } = 111;
        public int CurrentInstructionCount { get; set; } = 11;
        public int MaxCallChainDepth { get; set; } = 1;
        public int CurrentCallChainDepth { get; set; } = 1;
        public UpdateFrequency UpdateFrequency { get; set; } = UpdateFrequency.Update100;
    }
}
