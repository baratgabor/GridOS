using System;
using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    interface IGridService
    {
        void GetAllOfType<T>(List<T> listToPopulate, Func<T, bool> predicate = null) where T : class;
        IEnumerable<T> GetAllOfType<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class;
        IEnumerable<IMyTerminalBlock> GetAllTerminalBlocksOfType<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class;
        void GetAllTerminalBlocksOfType<T>(List<IMyTerminalBlock> listToPopulate, Func<IMyTerminalBlock, bool> predicate = null) where T : class;
        IMyTerminalBlock GetByName(string name);
        T GetFirst<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class;
        T GetFirstOrDefault<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class;
    }

    class GridService : IGridService
    {
        private readonly IMyGridTerminalSystem _gts;
        private readonly List<IMyTerminalBlock> _blockBuffer = new List<IMyTerminalBlock>();

        public GridService(IMyGridTerminalSystem gts)
        {
            _gts = gts;
        }

        public T GetFirst<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class
        {
            _blockBuffer.Clear();
            _gts.GetBlocksOfType<T>(_blockBuffer, predicate);
            return (T)_blockBuffer.First();
        }

        public T GetFirstOrDefault<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class
        {
            _blockBuffer.Clear();
            _gts.GetBlocksOfType<T>(_blockBuffer, predicate);
            return (T)_blockBuffer.FirstOrDefault();
        }

        public void GetAllOfType<T>(List<T> listToPopulate, Func<T, bool> predicate = null) where T : class
        {
            _gts.GetBlocksOfType(listToPopulate, predicate);
        }

        public IEnumerable<T> GetAllOfType<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class
        {
            _blockBuffer.Clear();
            _gts.GetBlocksOfType<T>(_blockBuffer, predicate);

            foreach (var item in _blockBuffer)
            {
                yield return (T)item;
            }
        }

        public void GetAllTerminalBlocksOfType<T>(List<IMyTerminalBlock> listToPopulate, Func<IMyTerminalBlock, bool> predicate = null) where T : class
        {
            _gts.GetBlocksOfType<T>(listToPopulate, predicate);
        }

        public IEnumerable<IMyTerminalBlock> GetAllTerminalBlocksOfType<T>(Func<IMyTerminalBlock, bool> predicate = null) where T : class
        {
            _blockBuffer.Clear();
            _gts.GetBlocksOfType<T>(_blockBuffer, predicate);
            return _blockBuffer;
        }

        public IMyTerminalBlock GetByName(string name)
        {
            return _gts.GetBlockWithName(name);
        }
    }
}
