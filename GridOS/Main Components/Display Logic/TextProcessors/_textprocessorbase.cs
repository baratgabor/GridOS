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
        interface ITextProcessor
        {
            StringBuilder Process(string input, ProcessingArgs args);
            StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false);
            // Implement this later; needs IndexOfAny implementation for StringBuilder:
            // StringBuilder Process(StringBuilder input, StringBuilder output, bool clearOutput = false);
        }
    }
}
