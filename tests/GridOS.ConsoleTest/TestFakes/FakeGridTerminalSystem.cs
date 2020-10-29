using System;
using System.Collections.Generic;

namespace IngameScript
{
    class FakeGridTerminalSystem : IMyGridTerminalSystem
    {
        public void GetBlocks(List<IMyTerminalBlock> blocks)
        {
            throw new NotImplementedException();
        }

        public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class
        {
            
        }

        public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class
        {
            
        }

        public IMyTerminalBlock GetBlockWithId(long id)
        {
            return null;
        }

        public IMyTerminalBlock GetBlockWithName(string name)
        {
            return null;
        }

        public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            
        }
    }
}
