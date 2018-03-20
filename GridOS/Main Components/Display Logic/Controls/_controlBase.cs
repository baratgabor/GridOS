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
            StringBuilder GetContent();
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

            public void Get_Process_Redraw()
            {
                Process_Redraw(_inner.GetContent());
            }

            protected void Process_Redraw(StringBuilder input)
            {
                _buffer = Process(input);
                RedrawRequired?.Invoke(_buffer);
            }

            public StringBuilder GetContent()
            {
                return _buffer;
            }

            protected abstract StringBuilder Process(StringBuilder input);
        }

        interface IScrollable : IControl
        {
            int LineNumber { get; }
            int VerticalOffset { get; }
            int LineHeight { get; }
            List<LineInfo> LineInfo { get; }
            bool SetVerticalOffset(int value);
        }

        // Yeah, ugly
        interface ILineInfoProviderControl : IControl
        {
            List<LineInfo> LineInfo { get; }
        }
    }
}
