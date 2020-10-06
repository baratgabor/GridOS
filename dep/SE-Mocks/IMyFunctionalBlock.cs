using System;

namespace IngameScript
{
    public interface IMyFunctionalBlock : IMyTerminalBlock, IMyCubeBlock, IMyEntity
    {
        bool Enabled { get; set; }

        [Obsolete("Use the setter of Enabled")]
        void RequestEnable(bool enable);
    }
}
