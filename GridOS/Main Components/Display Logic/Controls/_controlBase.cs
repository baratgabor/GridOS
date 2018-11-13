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
        interface IControl
        {
            event Action<StringBuilder> RedrawRequired;
            StringBuilder GetContent(bool FlushCache = false);
        }

        abstract class Frame : IControl
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IControl _inner;

            public event Action<StringBuilder> RedrawRequired;

            public Frame(IControl inner)
            {
                _inner = inner;
                _inner.RedrawRequired += Process_Redraw;
            }

            protected virtual void Redraw()
            {
                RedrawRequired?.Invoke(_buffer);
            }

            protected void Fetch(bool flush = false)
            {
                _buffer.Clear().Append(_inner.GetContent(flush));
            }

            protected void Fetch_Process(bool flush = false)
            {
                Process(_inner.GetContent(flush));
            }

            public void Fetch_Process_Redraw(bool flush = false)
            {
                Process_Redraw(_inner.GetContent(flush));
            }

            protected void Process_Redraw(StringBuilder input)
            {
                Process(input);
                Redraw();
            }

            public StringBuilder GetContent(bool FlushCache = false)
            {
                if (!FlushCache)
                    return _buffer;

                return Process(_inner.GetContent(true));
            }

            protected abstract StringBuilder Process(StringBuilder input);
        }

        interface IScrollable : IControl
        {
            int ContentLength { get; }
            int VerticalOffset { get; }
            int ViewportHeight { get; }
            List<LineInfo> LineInfo { get; }
            bool SetVerticalOffset(int value, bool redraw = true);
            bool ScrollToLine(int lineNumber, bool redraw = true);
        }

        // Yeah, ugly
        interface ILineInfoProviderControl : IControl
        {
            List<LineInfo> LineInfo { get; }
        }
    }
}
