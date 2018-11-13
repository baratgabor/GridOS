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
        class DisplayElementProcessor : IProcessor<IDisplayElement, string>
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected List<IDisplayElementProcessor> _pipeline = new List<IDisplayElementProcessor>();

            public DisplayElementProcessor(params IDisplayElementProcessor[] textProcessors)
            {
                _pipeline.AddRange(textProcessors);
            }

            public string Process(IDisplayElement displayElement)
            {
                // Prepare buffer for processing by populating it with the initial value
                _buffer
                    .Clear()
                    .Append(displayElement.Label);

                // Process the initial value in the buffer incrementally by each processor
                foreach (var p in _pipeline)
                    p.Process(_buffer, displayElement);

                return _buffer.ToString();
            }
        }
    }
}
