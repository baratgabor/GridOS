using System;

namespace IngameScript
{
    //
    // Summary:
    //     Flags set how often the script will run itself.
    [Flags]
    public enum UpdateFrequency : byte
    {
        //
        // Summary:
        //     Does not run autonomously.
        None = 0,
        //
        // Summary:
        //     Run every game tick.
        Update1 = 1,
        //
        // Summary:
        //     Run every 10th game tick.
        Update10 = 2,
        //
        // Summary:
        //     Run every 100th game tick.
        Update100 = 4,
        //
        // Summary:
        //     Run once before the next tick. Flag is un-set automatically after the update
        Once = 8
    }
}
