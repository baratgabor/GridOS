using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IngameScript
{
    interface IGridService
    {
        void GetAll<T>(List<T> listToPopulate, Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock;
        List<T> GetAll<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock;
        IMyTerminalBlock GetByName(string name);
        T GetFirst<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock;
        T GetFirstOrDefault<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock;
    }

    class GridService : IGridService
    {
        private readonly IMyGridTerminalSystem _gts;
        private readonly List<IMyTerminalBlock> _blockBuffer;


        public GridService(IMyGridTerminalSystem gts)
        {
            _gts = gts;
        }

        public T GetFirst<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock
        {
            _blockBuffer.Clear();
            _gts.GetBlocksOfType<T>(_blockBuffer, predicate);
            return (T)_blockBuffer.First();
        }

        public T GetFirstOrDefault<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock
        {
            _blockBuffer.Clear();
            _gts.GetBlocksOfType<T>(_blockBuffer, predicate);
            return (T)_blockBuffer.FirstOrDefault();
        }

        public void GetAll<T>(List<T> listToPopulate, Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock
        {
            _gts.GetBlocksOfType<T>(listToPopulate, predicate);
        }

        public List<T> GetAll<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class, IMyTerminalBlock
        {
            var blockList = new List<T>();
            _gts.GetBlocksOfType<T>(blockList, predicate);
            return blockList;
        }

        public IMyTerminalBlock GetByName(string name)
        {
            return _gts.GetBlockWithName(name);
        }
    }
}
