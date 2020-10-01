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
            public int LineHeight => _config.LineHeight;
            public int LineNumber => _source.LineInfo.Count;
            public List<LineInfo> LineInfo => _source.LineInfo;

            protected int _verticalOffset = 0;
            protected IMenuPresentationConfig _config;
            protected ILineInfoProviderControl _source;

            public ScrollableFrame(IMenuPresentationConfig config, ILineInfoProviderControl source) : base(source)
            {
                _config = config;
                _source = source;
            }

            public bool SetVerticalOffset(int value, bool redraw = true)
            {
                if (value == _verticalOffset || value < 0 || value + _config.LineHeight > LineInfo.Count)
                    return false;

                _verticalOffset = value;
                if (redraw) Fetch_Process_Redraw();
                return true;
            }

            protected override StringBuilder Process(StringBuilder input)
            {
                _buffer.Clear();

                int ViewportStart = LineInfo[_verticalOffset].StartPosition;
                int ViewportEnd = _verticalOffset + _config.LineHeight >= LineInfo.Count ? input.Length : LineInfo[_verticalOffset + _config.LineHeight].StartPosition - 2; // -2 cuts \r\n
                int ViewportLength = ViewportEnd - ViewportStart;

                _buffer.Append(input.ToString(ViewportStart, ViewportLength));
                return _buffer;
            }
        }
    }
}
