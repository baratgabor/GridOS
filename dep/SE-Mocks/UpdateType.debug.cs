using System;

namespace IngameScript
{
    //
    // Summary:
    //     Enum describes what source triggered the script to run.
    [Flags]
    public enum UpdateType
    {
        None = 0,
        //
        // Summary:
        //     Script run by user in the terminal.
        Terminal = 1,
        //
        // Summary:
        //     Script run by a block such as timer, sensor.
        Trigger = 2,
        //
        // Summary:
        //     Script run by a mod.
        Mod = 8,
        //
        // Summary:
        //     Script run by another programmable block.
        Script = 16,
        //
        // Summary:
        //     Script is updating every tick.
        Update1 = 32,
        //
        // Summary:
        //     Script is updating every 10th tick.
        Update10 = 64,
        //
        // Summary:
        //     Script is updating every 100th tick.
        Update100 = 128,
        //
        // Summary:
        //     Script is updating once before the tick.
        Once = 256,
        //
        // Summary:
        //     Script run by intergrid communication system.
        IGC = 512
    }
}
