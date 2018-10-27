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
        class AddPrefix : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _buffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                output.Append(args.Prefix + (args.Prefix.Length > 0 ? " " : "") + input);
                return output;
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                if (args.Prefix.Length > 0)
                    inputOutput.Insert(0, args.Prefix + " ");
            }
        }
    }
}
