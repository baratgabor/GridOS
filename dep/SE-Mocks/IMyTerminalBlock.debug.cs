using System.Text;
using System;

namespace IngameScript
{
    public interface IMyTerminalBlock : IMyCubeBlock, IMyEntity
    {
        bool ShowInInventory { get; set; }
        bool ShowInTerminal { get; set; }
        bool ShowOnHUD { get; set; }
        //
        // Summary:
        //     Gets or sets the Custom Data string. NOTE: Only use this for user input. For
        //     storing large mod configs, create your own MyModStorageComponent
        string CustomData { get; set; }
        string CustomInfo { get; }
        bool ShowInToolbarConfig { get; set; }
        string CustomName { get; set; }
        string CustomNameWithFaction { get; }
        string DetailedInfo { get; }

        //void GetActions(List<ITerminalAction> resultList, Func<ITerminalAction, bool> collect = null);
        //ITerminalAction GetActionWithName(string name);
        //void GetProperties(List<ITerminalProperty> resultList, Func<ITerminalProperty, bool> collect = null);
        //ITerminalProperty GetProperty(string id);
        bool HasLocalPlayerAccess();
        bool HasPlayerAccess(long playerId);
        //
        // Summary:
        //     Determines whether this block is mechanically connected to the other. This is
        //     any block connected with rotors or pistons or other mechanical devices, but not
        //     things like connectors. This will in most cases constitute your complete construct.
        //     Be aware that using merge blocks combines grids into one, so this function will
        //     not filter out grids connected that way. Also be aware that detaching the heads
        //     of pistons and rotors will cause this connection to change.
        //
        // Parameters:
        //   other:
        bool IsSameConstructAs(IMyTerminalBlock other);
        //void SearchActionsOfName(string name, List<ITerminalAction> resultList, Func<ITerminalAction, bool> collect = null);
        [Obsolete("Use the setter of Customname")]
        void SetCustomName(StringBuilder text);
        [Obsolete("Use the setter of Customname")]
        void SetCustomName(string text);
    }
}
