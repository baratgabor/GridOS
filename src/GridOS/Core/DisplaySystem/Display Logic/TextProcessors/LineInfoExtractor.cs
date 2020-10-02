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
        // TODO: In dire need of refactor. This class' Process() method actually doesn't do what it seems to promise, but instead it does side effects: it fills the passed in 'args' ProcessingArgs instance

        /// <summary>
        /// Extracts data needed for higher level components.
        /// </summary>
        class LineInfoExtractor : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IMenuPresentationConfig _config;

            public LineInfoExtractor(IMenuPresentationConfig config)
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
                // also assumes that structure is "padding + prefix + item", but actually order can be any
                int bulletCharPosition = args.CurrentOutputLength + _config.PaddingLeft;

                for (int currentPos = 0; currentPos != -1; currentPos = input.IndexOf(Environment.NewLine, currentPos))
                {
                    if (currentPos != -1)
                    {
                        if (currentPos != 0) currentPos += Environment.NewLine.Length;
                        args.LineInfo.Add(new LineInfo(currentPos + args.CurrentOutputLength, args.Item, bulletCharPosition));
                        currentPos++;
                    }
                }

                return output;
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                Process(inputOutput.ToString(), args, inputOutput);
            }
        }
    }
}
