using System;

namespace IngameScript
{
    //
    // Summary:
    //     The interface for the grid program provides extra access for the game and for
    //     mods. See Sandbox.ModAPI.Ingame.MyGridProgram for the class the scripts actually
    //     derive from.
    public interface IMyGridProgram
    {
        //Func<IMyIntergridCommunicationSystem> IGC_ContextGetter { set; }
        //
        // Summary:
        //     Gets or sets the GridTerminalSystem available for the grid programs.
        IMyGridTerminalSystem GridTerminalSystem { get; set; }
        //
        // Summary:
        //     Gets or sets the programmable block which is currently running this grid program.
        //IMyProgrammableBlock Me { get; set; }
        //
        // Summary:
        //     Gets or sets the amount of time elapsed since the last time this grid program
        //     was run.
        [Obsolete("Use Runtime.TimeSinceLastRun instead")]
        TimeSpan ElapsedTime { get; set; }
        //
        // Summary:
        //     Gets or sets the storage string for this grid program.
        string Storage { get; set; }
        //
        // Summary:
        //     Gets or sets the object used to provide runtime information for the running grid
        //     program.
        IMyGridProgramRuntimeInfo Runtime { get; set; }
        //
        // Summary:
        //     Gets or sets the action which prints out text onto the currently running programmable
        //     block's detail info area.
        Action<string> Echo { get; set; }
        //
        // Summary:
        //     Determines whether this grid program has a valid Main method.
        bool HasMainMethod { get; }
        //
        // Summary:
        //     Determines whether this grid program has a valid Save method.
        bool HasSaveMethod { get; }

        //
        // Summary:
        //     Invokes this grid program.
        //
        // Parameters:
        //   argument:
        [Obsolete("Use overload Main(String, UpdateType)")]
        void Main(string argument);
        //
        // Summary:
        //     Invokes this grid program with the given update source.
        //
        // Parameters:
        //   argument:
        //
        //   updateSource:
        void Main(string argument, UpdateType updateSource);
        //
        // Summary:
        //     If this grid program has state saving capability, calling this method will invoke
        //     it.
        void Save();
    }
}
