﻿using Sandbox.Game.EntityComponents;
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
            public event Action<IDisplayElement> ItemSelected;

            public NavigationFrame(INavConfig config, IScrollable scrollableBox) : base(scrollableBox)
            {
                _config = config;
                _scrollableBox = scrollableBox;
            }

            protected bool IsReady()
            {
                if (_buffer.Length == 0)
                    return false;

                return true;
            }

            public void MoveUp(CommandItem sender, string parameter)
            {
                MoveUp();
            }

            public bool MoveUp()
            {
                if (!IsReady())
                    return false;

                if (_selectedLine <= 0)
                    return false;
                AdjustSelectedLineAndDraw(_selectedLine - 1);
                return true;
            }

            public void MoveDown(CommandItem sender, string parameter)
            {
                MoveDown();
            }

            public bool MoveDown()
            {
                if (!IsReady())
                    return false;

                if (_selectedLine >= _scrollableBox.LineNumber - 1)
                    return false;

                AdjustSelectedLineAndDraw(_selectedLine + 1);
                return true;
            }

            public void Select(CommandItem sender, string parameter)
            {
                Select();
            }

            public bool Select()
            {
                if (!IsReady())
                    return false;

                if (_selectedLine > _scrollableBox.LineInfo.Count - 1)
                    return false;

                ItemSelected?.Invoke(_scrollableBox.LineInfo[_selectedLine].ParentDisplayElement);
                return true;
            }

            protected void AdjustSelectedLineAndDraw(int newSelectedLine)
            {
                if (AdjustVerticalOffset(newSelectedLine) == false)
                {
                    RemoveSelectionMarker(_selectedLine);
                    AddSelectionMarker(newSelectedLine);
                    _selectedLine = newSelectedLine;
                    Redraw();
                }
            }

            protected void AddSelectionMarker(int selectedLine)
            {
                _buffer[_scrollableBox.LineInfo[selectedLine].StartPosition + 1] = _config.SelectionMarker;
            }

            protected void RemoveSelectionMarker(int selectedLine)
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
                AdjustSelectedLineAndDraw(_selectedLine);

                return _buffer;
            }
        }
    }
}
