using System;

namespace IngameScript
{
    //
    // Summary:
    //     Basic cube interface
    public interface IMyCubeBlock : IMyEntity
    {
        //
        // Summary:
        //     Translated block name
        string DisplayNameText { get; }
        //
        // Summary:
        //     Returns block orientation in base 6 directions
        //MyBlockOrientation Orientation { get; }
        //
        // Summary:
        //     Order in which were the blocks of same type added to grid Used in default display
        //     name
        int NumberInGrid { get; }
        //
        // Summary:
        //     Minimum coordinates of grid cells occupied by this block
        //Vector3I Min { get; }
        //
        // Summary:
        //     Block mass
        float Mass { get; }
        //
        // Summary:
        //     Maximum coordinates of grid cells occupied by this block
        //Vector3I Max { get; }
        //
        // Summary:
        //     True if block is able to do its work depening on block type (is functional, powered,
        //     enabled, etc...)
        bool IsWorking { get; }
        //
        // Summary:
        //     True if integrity is above breaking threshold
        bool IsFunctional { get; }
        //
        // Summary:
        //     Hacking of the block is in progress
        bool IsBeingHacked { get; }
        //
        // Summary:
        //     Id of player owning block (not steam Id)
        long OwnerId { get; }
        //
        // Summary:
        //     Position in grid coordinates
        //Vector3I Position { get; }
        //
        // Summary:
        //     Definition name
        string DefinitionDisplayNameText { get; }
        //
        // Summary:
        //     Grid in which the block is placed
        IMyCubeGrid CubeGrid { get; }
        bool CheckConnectionAllowed { get; }
        //SerializableDefinitionId BlockDefinition { get; }
        //
        // Summary:
        //     Is set in definition Ratio at which is the block disassembled (grinding)
        float DisassembleRatio { get; }

        //
        // Summary:
        //     Tag of faction owning block
        string GetOwnerFactionTag();
        //[Obsolete("GetPlayerRelationToOwner() is useless ingame. Mods should use the one in ModAPI.IMyCubeBlock")]
        //MyRelationsBetweenPlayerAndBlock GetPlayerRelationToOwner();
        //MyRelationsBetweenPlayerAndBlock GetUserRelationToOwner(long playerId);
        [Obsolete]
        void UpdateIsWorking();
        [Obsolete]
        void UpdateVisual();
    }
}
