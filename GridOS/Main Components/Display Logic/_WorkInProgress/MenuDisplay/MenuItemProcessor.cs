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
        class MenuItemProcessor : IProcessor<IMenuItem, string>
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected List<IMenuItemProcessor> _pipeline = new List<IMenuItemProcessor>();

            public MenuItemProcessor(params IMenuItemProcessor[] itemProcessors)
            {
                _pipeline.AddRange(itemProcessors);
            }

            public string Process(IMenuItem menuItem)
            {
                // Prepare buffer for processing by populating it with the initial value
                _buffer
                    .Clear()
                    .Append(menuItem.Label);

                // Process the initial value in the buffer incrementally by each processor
                foreach (var p in _pipeline)
                    p.Process(_buffer, menuItem);

                return _buffer.ToString();
            }
        }
    }
}
