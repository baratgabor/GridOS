using System.Collections.Generic;
using System;

namespace IngameScript
{
    public interface IMyGridTerminalSystem
    {
        //void GetBlockGroups(List<IMyBlockGroup> blockGroups, Func<IMyBlockGroup, bool> collect = null);
        //IMyBlockGroup GetBlockGroupWithName(string name);
        void GetBlocks(List<IMyTerminalBlock> blocks);
        void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class;
        void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class;
        IMyTerminalBlock GetBlockWithId(long id);
        IMyTerminalBlock GetBlockWithName(string name);
        void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null);
    }
}
