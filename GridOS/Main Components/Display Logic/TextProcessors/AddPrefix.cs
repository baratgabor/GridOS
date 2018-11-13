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
        /// Simply adds a prefix to the input.
        /// </summary>
        class AddPrefix : IDisplayElementProcessor
        {
            protected IAffixConfig _config;

            public AddPrefix(IAffixConfig config)
            {
                _config = config;
            }

            public void Process(StringBuilder processable, IDisplayElement referenceDisplayElement)
            {
                var prefix = _config.GetPrefixFor(referenceDisplayElement, false);
                if (prefix.Length == 0)
                    return;
                processable.Insert(0, prefix + " ");
            }
        }
    }
}
