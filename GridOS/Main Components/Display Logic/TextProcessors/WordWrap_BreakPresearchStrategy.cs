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
        /// Wraps the provided string or StringBuilder according to the settings given.
        /// Break-presearch strategy: proactively searches for first breakable character, copies substrings instead of char array iteration.
        /// </summary>
        class WordWrap_BreakPresearchStrategy : ITextProcessor
        {
            protected StringBuilder _inputBuffer = new StringBuilder();
            protected StringBuilder _outputBuffer = new StringBuilder();
            protected IWordWrappingConfig _config;

            public WordWrap_BreakPresearchStrategy(IWordWrappingConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _outputBuffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                int prevBreakPos = 0;
                foreach (var breakPos in FindNextLineBreak(input, _config.LineLength))
                {
                    output.Append(input.Substring(prevBreakPos, breakPos - prevBreakPos) + Environment.NewLine);
                    prevBreakPos = breakPos;
                }
                // TODO: Take note that this creates new strings for each line; might not be an issue with caching, but could worth replacing it
                output.Append(input.Substring(prevBreakPos, input.Length - prevBreakPos)); // Rest of string until the end
                return output;
            }

            protected IEnumerable<int> FindNextLineBreak(string input, int lineLength)
            {
                for (int currentPos = 0, lastBreakPos = 0, lastGoodPos = 0; currentPos != -1; currentPos = input.IndexOfAny(_config.Terminators, currentPos))
                {
                    if (currentPos != -1 && currentPos >= lastBreakPos + lineLength)
                    {
                        lastBreakPos = lastGoodPos;
                        yield return lastGoodPos + 1; // +1 to put terminator at the end of line, instead of at the beginning of next line
                    }
                    lastGoodPos = currentPos;
                    if (currentPos != -1) currentPos++; // Skips space; otherwise IndexOf() returns the same value over and over again.
                }
            }
        }
    }
}
