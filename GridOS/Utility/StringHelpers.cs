using System.Collections.Generic;
using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        public class StringHelpers
        {
            public static IEnumerable<StringSegment> WordWrap(string input, int maxLineLength, char[] breakableCharacters)
            {
                int lastBreakIndex = 0;

                while (true)
                {
                    var nextForcedLineBreak = lastBreakIndex + maxLineLength;

                    // If the remainder is shorter than the allowed line-length, return the remainder. Short-circuits instantly for strings shorter than line-length.
                    if (nextForcedLineBreak >= input.Length)
                    {
                        yield return new StringSegment(input, lastBreakIndex, input.Length - lastBreakIndex);
                        yield break;
                    }

                    // If there are native new lines before the next forced break position, use the last native new line as the starting position of our next line.
                    int nativeNewlineIndex = input.LastIndexOf(Environment.NewLine, nextForcedLineBreak, maxLineLength);
                    if (nativeNewlineIndex > -1)
                    {
                        nextForcedLineBreak = nativeNewlineIndex + Environment.NewLine.Length + maxLineLength;
                    }

                    // Find the last breakable point preceding the next forced break position (and include the breakable character, which might be a hypen).
                    var nextBreakIndex = input.LastIndexOfAny(breakableCharacters, nextForcedLineBreak, maxLineLength) + 1;

                    // If there is no breakable point, which means a word is longer than line length, force-break it.
                    if (nextBreakIndex == 0)
                    {
                        nextBreakIndex = nextForcedLineBreak;
                    }

                    yield return new StringSegment(input, lastBreakIndex, nextBreakIndex - lastBreakIndex);

                    lastBreakIndex = nextBreakIndex;
                }
            }
        }
    }
}
