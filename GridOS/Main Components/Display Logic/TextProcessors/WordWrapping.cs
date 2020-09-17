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
        /// Breaks the provided content into lines according to the configured line length.
        /// </summary>
        class WordWrapping : IMenuItemProcessor
        {
            protected IWordWrappingConfig _config;

            public WordWrapping(IWordWrappingConfig config)
            {
                _config = config;
            }

            public void Process(StringBuilder processable, IMenuItem referenceMenuItem)
            {
                Process_inner(processable.ToString(), processable.Clear(), _config.LineLength, _config.Terminators);
            }

            protected void Process_inner(string input, StringBuilder output, int lineLength, char[] breakableCharacters)
            {
                int prevBreakPos = 0;
                foreach (var breakPos in DetermineNextLineBreak(input, lineLength, breakableCharacters))
                {
                    output.Append(input, prevBreakPos, breakPos - prevBreakPos);
                    output.Append(Environment.NewLine);
                    prevBreakPos = breakPos;
                }
                // Add rest of string until the end
                output.Append(input, prevBreakPos, input.Length - prevBreakPos);
            }

            protected IEnumerable<int> DetermineNextLineBreak(string input, int lineLength, char[] breakableCharacters)
            {
                int currentPosition = 0;

                while (true)
                {
                    int nextForcedBreakPosition = currentPosition + lineLength;

                    // Case 1: The whole string is simply shorter than line length (breaks out instantly at first iteration)
                    // Case 2: The end of the string is shorter than line length (break out at last iteration)
                    if (nextForcedBreakPosition >= input.Length)
                        break;

                    // Search for native new line in the string within the maximum scope of the current line
                    int nativeNewLine = input.IndexOf(Environment.NewLine, currentPosition, lineLength);
                    // If found, simply skip into after the new line, and start from the beginning
                    if (nativeNewLine != -1)
                    {
                        currentPosition = nativeNewLine + Environment.NewLine.Length;
                        continue;
                    }

                    // Search for the first breakable character, going backwards from the position where the next line must break
                    int breakablePoint = input.LastIndexOfAny(breakableCharacters, nextForcedBreakPosition, lineLength);
                    // If found, yield at the found position
                    if (breakablePoint != -1)
                        currentPosition = breakablePoint + 1; // +1 adds the detected character itself to the end of line
                    // If not found, yield at the forced break position (this breaks up 'words' longer than line length)
                    else
                        currentPosition = nextForcedBreakPosition;

                    yield return currentPosition;
                }
            }
        }
    }
}
