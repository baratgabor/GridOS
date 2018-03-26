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
            protected int _startingPosition => _scrollableBox.LineInfo[_scrollableBox.VerticalOffset].StartPosition;
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


            // TODO: Factor out this hack; think about some more flexible flow control.
            // This hack causes double-processing of content here in NavigationFrame:
            // 1) GetContent(true) will process content (with selection 0) without drawing
            // 2) Then, if _selectedLine changes, AdjustVerticalOffset will cause a re-process
            public void OnPathChanged(ContentChangeInfo obj)
            {
                _selectedLine = 0;
                _scrollableBox.SetVerticalOffset(0, false);
                // Pull new content from bottom of chain and process - this refreshes the LineInfo we need below
                GetContent(true);

                if (obj.PreviousContext != null)
                {
                    var lineInfo = _scrollableBox.LineInfo.FirstOrDefault(x => x.ParentDisplayElement == obj.PreviousContext);
                    if (!object.Equals(lineInfo, default(LineInfo)))
                    {
                        _selectedLine = _scrollableBox.LineInfo.IndexOf(lineInfo);
                    }
                }

                // If selected line changed, try adjusting offset (which will redraw if offset is adjusted)
                bool redrawOccurred = false;
                if (_selectedLine != 0)
                    redrawOccurred = AdjustVerticalOffset(_selectedLine);

                // If selectedline didn't change, or it did, but no adjustment was needed, we need a manual redraw
                if (_selectedLine == 0 || !redrawOccurred)
                    Redraw();
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
                _buffer[_scrollableBox.LineInfo[selectedLine].StartPosition + 1 - _startingPosition] = _config.SelectionMarker;
            }

            protected void RemoveSelectionMarker(int selectedLine)
            {
                _buffer[_scrollableBox.LineInfo[selectedLine].StartPosition + 1 - _startingPosition] = ' ';
            }

            protected bool AdjustVerticalOffset(int selectedLine)
            {
                int vo = _scrollableBox.VerticalOffset;
                int lh = _scrollableBox.LineHeight;
                int max_scroll = _scrollableBox.LineNumber - lh;

                if (selectedLine < vo)
                {
                    _selectedLine = selectedLine;
                    _scrollableBox.SetVerticalOffset(vo - 1);
                    return true;
                }
                else if (selectedLine + 2 > vo + lh && vo + lh < _scrollableBox.LineNumber)
                {
                    _selectedLine = selectedLine;
                    _scrollableBox.SetVerticalOffset((selectedLine + 2 - lh > max_scroll ? max_scroll : selectedLine + 2 - lh));
                    return true;
                }

                return false;
            }

            protected override StringBuilder Process(StringBuilder input)
            {
                _buffer.Clear();
                _buffer.Append(input);

                AddSelectionMarker(_selectedLine);

                return _buffer;
            }
        }
    }
}
