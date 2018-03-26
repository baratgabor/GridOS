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
        class DisplayHeader : IControl
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected ProgressIndicator2 _spinner = new ProgressIndicator2();
            IMyGridProgramRuntimeInfo _runtime;

            public event Action<StringBuilder> RedrawRequired;

            public DisplayHeader(IBreadcrumbConfig config, IMyGridProgramRuntimeInfo runtime)
            {
                _runtime = runtime;
            }

            public StringBuilder GetContent(bool FlushCache = false)
            {
                _buffer.Clear();
                _buffer.Append($"{Environment.NewLine}  ::GridOS:: {_spinner.Get()} LRT: {_runtime.LastRunTimeMs:G3}ms");
                return _buffer;
            }
        }
    }
}
