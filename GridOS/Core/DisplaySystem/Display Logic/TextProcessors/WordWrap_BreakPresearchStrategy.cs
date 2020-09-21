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
        // TODO: Implement case for words longer than a line length without any breakable part
        /// <summary>
        /// Wraps the provided string or StringBuilder according to the settings given.
        /// Break-presearch strategy: proactively searches for first breakable character, copies substrings instead of char array iteration.
        /// </summary>
        class WordWrap_BreakPresearchStrategy : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IWordWrappingConfig _config;

            public WordWrap_BreakPresearchStrategy(IWordWrappingConfig config)
            {
                _config = config;
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                Process(inputOutput, args, _buffer, true); // result will be in _buffer
                inputOutput
                    .Clear()
                    .Append(_buffer);
            }

            public StringBuilder Process(StringBuilder input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                int prevBreakPos = 0;
                foreach (var breakPos in FindNextLineBreak(input, _config.LineLength))
                {
                    // TODO: Take note that this allocates new string for each line; might not be an issue with caching, but could worth replacing it
                    output.Append(input.ToString(), prevBreakPos, breakPos - prevBreakPos);
                    output.Append(Environment.NewLine);
                    prevBreakPos = breakPos;
                }
                
                output.Append(input.ToString(), prevBreakPos, input.Length - prevBreakPos); // Rest of string until the end
                return output;
            }

            protected IEnumerable<int> FindNextLineBreak(StringBuilder input, int lineLength)
            {
                bool checkNativeNewLines = true;
                for (int currentPos = 0, lastBreakPos = 0, lastGoodPos = 0; ; currentPos = input.IndexOfAny(_config.Terminators, currentPos))
                {
                    // Yield a breakpoint at the last valid breakable point, in two cases:
                    // 1) Next breakable point would exceed line length
                    // 2) No next breakable point, but last word would exceed line length
                    if ((currentPos != -1 && currentPos >= lastBreakPos + lineLength) ||
                        (currentPos == -1 && lastBreakPos + lineLength < input.Length))
                    {
                        lastBreakPos = lastGoodPos;
                        yield return lastGoodPos + 1; // +1 to put terminator at the end of line, instead of at the beginning of next line
                        checkNativeNewLines = true;
                    }
                    // Break out if there are no more breakable points
                    if (currentPos == -1)
                        break;

                    // Special case for new lines already existing in the input text:
                    // If such a new line is found in the range of the current line
                    // (i.e. from last line break to (last line break + line length) OR until end of string)
                    // then jump to that position and start counting our line from there.
                    if (checkNativeNewLines)
                    {
                        int nativeNewlinePos = input.IndexOf(Environment.NewLine, lastBreakPos, (lineLength < input.Length - lastBreakPos ? lineLength : input.Length - lastBreakPos), false);
                        if (nativeNewlinePos == -1)
                            checkNativeNewLines = false; // If fails once, don't repeat IndexOf() checking until the next line
                        // Exclude newlines at the end of string 
                        else if (nativeNewlinePos + Environment.NewLine.Length < input.Length)
                        {
                            currentPos = nativeNewlinePos + Environment.NewLine.Length;
                            lastBreakPos = nativeNewlinePos + Environment.NewLine.Length;
                        }
                    }

                    lastGoodPos = currentPos;
                    if (currentPos != -1) currentPos++; // Skips matched char; otherwise IndexOf() returns the same value over and over again.}
                }
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                throw new NotImplementedException();
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                throw new NotImplementedException();
            }

        }
    }
}
