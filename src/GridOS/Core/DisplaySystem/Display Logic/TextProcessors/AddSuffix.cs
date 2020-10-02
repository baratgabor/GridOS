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
        /// Simply adds a suffix to the input.
        /// </summary>
        class AddSuffix : ITextProcessor
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

                output.Append(input);
                
                if (args.Suffix != ' ')
                {
                    output
                        .Append(' ')
                        .Append(args.Suffix);
                }

                return output;
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                inputOutput.Append(' ');
                inputOutput.Append(args.Suffix);
            }
        }
    }
}
