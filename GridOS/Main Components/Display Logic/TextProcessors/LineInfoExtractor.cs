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
        /// This text processor doesn't process anything. It extracts data needed for higher level components, with the help of the passed "args". Welcome to the Wild West.
        /// </summary>
        class LineInfoExtractor : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IPaddingConfig _config;

            public LineInfoExtractor(IPaddingConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                _buffer.Clear();
                _buffer.Append(input);
                return Process(input, args, _buffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                // TODO: highly assumptious; bulletCharPosition is problematic, since we switch to prefix strings, which can be longer than 1 char.
                // also assumes that structure is "padding + prefix + element", but actually order can be any
                int bulletCharPosition = args.CurrentOutputLength + _config.PaddingLeft;

                for (int currentPos = 0; currentPos != -1; currentPos = input.IndexOf(Environment.NewLine, currentPos))
                {
                    if (currentPos != -1)
                    {
                        if (currentPos != 0) currentPos += Environment.NewLine.Length;
                        args.LineInfo.Add(new LineInfo(currentPos + args.CurrentOutputLength, args.Element, bulletCharPosition));
                        currentPos++;
                    }
                }

                return output;
            }
        }
    }
}
