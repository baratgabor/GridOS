using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    class FakeGridTerminalSystem : IMyGridTerminalSystem
    {
        public List<IMyTerminalBlock> FakeBlocks { get; set; } = new List<IMyTerminalBlock>();

        public void GetBlocks(List<IMyTerminalBlock> blocks)
        {
            blocks.AddRange(FakeBlocks);
        }

        public void GetBlocksOfType<T>(List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null) where T : class
        {
            blocks.AddRange(FakeBlocks.Where(x => x is T && (collect == null || collect(x))));
        }

        public void GetBlocksOfType<T>(List<T> blocks, Func<T, bool> collect = null) where T : class
        {
            var list = FakeBlocks.OfType<T>();

            if (collect != null)
                list = list.Where(collect);

            blocks.AddRange(list);
        }

        public IMyTerminalBlock GetBlockWithId(long id)
        {
            return FakeBlocks.FirstOrDefault(x => x.EntityId == id);
        }

        public IMyTerminalBlock GetBlockWithName(string name)
        {
            return FakeBlocks.FirstOrDefault(x => x.DisplayName == name);
        }

        public void SearchBlocksOfName(string name, List<IMyTerminalBlock> blocks, Func<IMyTerminalBlock, bool> collect = null)
        {
            blocks.AddRange(FakeBlocks.Where(x => x.DisplayName == name && (collect == null || collect(x))));
        }
    }
}
