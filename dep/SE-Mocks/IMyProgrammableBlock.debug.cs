namespace IngameScript
{
    public interface IMyProgrammableBlock : IMyFunctionalBlock, IMyTerminalBlock, IMyCubeBlock, IMyEntity, IMyTextSurfaceProvider
    {
        //
        // Summary:
        //     This programmable block is currently running its program.
        bool IsRunning { get; }
        //
        // Summary:
        //     Contains the value of the default terminal argument.
        string TerminalRunArgument { get; }

        //
        // Summary:
        //     Attempts to run this programmable block using the given argument. An already
        //     running programmable block cannot be run again. This is equivalent to running
        //     block.ApplyAction("Run", argumentsList); This should be called from an ingame
        //     script. Do not use in mods.
        //
        // Parameters:
        //   argument:
        //
        // Returns:
        //     true if the action was applied, false otherwise
        bool TryRun(string argument);
    }
}
