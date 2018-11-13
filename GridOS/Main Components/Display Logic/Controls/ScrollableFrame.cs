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
        /// Scrollable wrapper around string content
        /// </summary>
        class ScrollableFrame : Frame, IScrollable
        {
            public int VerticalOffset => _verticalOffset;
            public int ContentLength => _source.LineInfo.Count;
            public int ViewportHeight => _config.LineHeight;
            public int ViewportStart => _verticalOffset;
            public int ViewportEnd => VerticalOffset + ViewportHeight;
            public List<LineInfo> LineInfo => _source.LineInfo;

            protected int _verticalOffset = 0;
            protected IViewportConfig _config;
            protected ILineInfoProviderControl _source;

            public ScrollableFrame(IViewportConfig config, ILineInfoProviderControl source) : base(source)
            {
                _config = config;
                _source = source;
            }

            public bool SetVerticalOffset(int value, bool redraw = true)
            {
                if (value == _verticalOffset || value < 0 || IsOverMaximumScroll(value))
                    return false;

                _verticalOffset = value;

                if (redraw) Fetch_Process_Redraw();

                return true;
            }

            protected override StringBuilder Process(StringBuilder input)
            {
                _buffer.Clear();

                int ViewportStartChar = LineInfo[_verticalOffset].StartPosition;
                int ViewportEndChar = _verticalOffset + _config.LineHeight >= LineInfo.Count ? input.Length : LineInfo[_verticalOffset + _config.LineHeight].StartPosition - 2; // -2 cuts \r\n
                int ViewportLengthChar = ViewportEndChar - ViewportStartChar;

                _buffer.Append(input.ToString(ViewportStartChar, ViewportLengthChar));
                return _buffer;
            }

            /// <summary>
            /// Ensures that given line is visible in viewport by adjusting the vertical offset, if needed.
            /// </summary>
            /// <param name="lineNumber">The line number to bring into the viewport.</param>
            /// <returns>True if adjustment was needed. False if the given line was already in viewport.</returns>
            public bool ScrollToLine(int lineNumber, bool redraw = true)
            {
                int max_scroll = ContentLength - ViewportHeight;

                // Line is above viewport
                if (lineNumber < VerticalOffset)
                {
                    SetVerticalOffset(lineNumber, redraw);
                    if (redraw == false)
                        Fetch_Process();
                    return true;
                }
                // Line is below viewport, and scrolling didn't yet reach the end of the content
                else if (lineNumber + 2 > ViewportEnd
                    && ViewportEnd < ContentLength)
                {
                    SetVerticalOffset((lineNumber + 2 - ViewportHeight > max_scroll ? max_scroll : lineNumber + 2 - ViewportHeight), redraw);
                    if (redraw == false)
                        Fetch_Process();
                    return true;
                }

                return false;
            }

            protected bool IsOverMaximumScroll(int lineNumber)
            {
                return lineNumber + ViewportHeight > ContentLength;
            }
        }
    }
}
