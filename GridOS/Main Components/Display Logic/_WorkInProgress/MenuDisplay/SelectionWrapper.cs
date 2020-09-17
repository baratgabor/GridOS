using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        class SelectionWrapper
        {
            protected MenuViewModel _menuViewModel;
            protected int _lastLine => _menuViewModel.LineHeight - 1;
            protected int _selectedLine;

            public SelectionWrapper(MenuViewModel menuViewModel)
            {
                _menuViewModel = menuViewModel;
            }

            public void MoveSelectionUp()
            {
                if (_selectedLine > 0)
                    _selectedLine--;
                else
                    _menuViewModel.MoveUp();
            }

            public void MoveSelectionDown()
            {
                if (_selectedLine < _lastLine)
                    _selectedLine++;
                else
                    _menuViewModel.MoveDown();
            }

            public void TryExecuteSelected()
            {
                var element = _menuViewModel.GetElementAt(_selectedLine);

                if (element is IMenuCommand menuCommand)
                    menuCommand.Execute();

            }
        }
    }
}
