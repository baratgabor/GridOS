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

namespace IngameScript
{
	partial class Program
	{
        /// <summary>
        /// Navigation layer on top of a scrollable box
        /// </summary>
        class NavigationFrame : Frame
        {
            protected INavConfig _config;
            protected int _selectedLine = 0;
            protected IScrollable _scrollableBox;

            public NavigationFrame(INavConfig config, IScrollable scrollableBox) : base(scrollableBox)
            {
                _config = config;
                _scrollableBox = scrollableBox;
            }

            public bool MoveUp()
            {
                if (_selectedLine <= 0)
                    return false;

                AdjustSelectedLine(_selectedLine - 1);
                return true;
            }

            public bool MoveDown()
            {
                if (_selectedLine >= _scrollableBox.LineNumber - 1)
                    return false;

                AdjustSelectedLine(_selectedLine + 1);
                return true;
            }

            private void AdjustSelectedLine(int value)
            {
                if (AdjustVerticalOffset(value) == false)
                {
                    RemoveSelectionMarker(_selectedLine);
                    AddSelectionMarker(value);
                    _selectedLine = value;
                }
            }

            private void AddSelectionMarker(int selectedLine)
            {
                _buffer[_scrollableBox.LineInfo[selectedLine].StartPosition + 1] = _config.SelectionMarker;
            }

            private void RemoveSelectionMarker(int selectedLine)
            {
                _buffer[_scrollableBox.LineInfo[selectedLine].StartPosition + 1] = ' ';
            }

            protected bool AdjustVerticalOffset(int selectedLine)
            {
                int vo = _scrollableBox.VerticalOffset;
                int lh = _scrollableBox.LineHeight;

                if (selectedLine < vo)
                {
                    _scrollableBox.SetVerticalOffset(vo - 1);
                    return true;
                }
                else if (selectedLine + 2 > vo + lh)
                {
                    _scrollableBox.SetVerticalOffset(selectedLine + 2 - lh);
                    return true;
                }

                return false;
            }

            protected override StringBuilder Process(StringBuilder input)
            {
                _buffer.Clear();
                _buffer.Append(input);

                // Needed after moving to a different "folder"?
                // AdjustVerticalOffset(_selectedLine);
                AddSelectionMarker(_selectedLine);

                return _buffer;
            }
        }
    }
}
