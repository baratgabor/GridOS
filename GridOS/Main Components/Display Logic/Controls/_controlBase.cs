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

            protected void Get_Process()
            {
                _buffer = Process(_inner.GetContent());
            }

            public void Fetch_Process_Redraw()
            {
                Process_Redraw(_inner.GetContent());
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
            int LineNumber { get; }
            int VerticalOffset { get; }
            int LineHeight { get; }
            List<LineInfo> LineInfo { get; }
            bool SetVerticalOffset(int value, bool redraw = true);
        }

        // Yeah, ugly
        interface ILineInfoProviderControl : IControl
        {
            List<LineInfo> LineInfo { get; }
        }
    }
}
