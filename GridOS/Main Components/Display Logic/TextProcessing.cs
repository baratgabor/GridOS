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
            StringBuilder Process(string input);
            StringBuilder Process(string input, StringBuilder output, bool clearOutput = false);
            // implement later: StringBuilder Process(StringBuilder input, StringBuilder output, bool clearOutput = false);
        }

        /// <summary>
        /// Wraps the provided string or StringBuilder according to the settings given.
        /// </summary>
        class WordWrap_Strategy1 : ITextProcessor
        {
            protected StringBuilder _inputBuffer = new StringBuilder();
            protected StringBuilder _outputBuffer = new StringBuilder();
            public int LineWidth { get; set; }
            protected char[] _terminators = { ' ', '-' };

            public StringBuilder Process(string input)
            {
                return Process(input, _outputBuffer, true);
            }

            public StringBuilder Process(string input, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                int prevBreakPos = 0;
                foreach (var breakPos in FindNextLineBreak(input, LineWidth))
                {
                    output.Append(input.Substring(prevBreakPos, breakPos - prevBreakPos) + Environment.NewLine);
                    prevBreakPos = breakPos;
                }
                output.Append(input.Substring(prevBreakPos, input.Length - prevBreakPos)); // Rest of string until the end
                return output;
            }

            protected IEnumerable<int> FindNextLineBreak(string input, int lineWidth)
            {
                for (int currentPos = 0, lastBreakPos = 0, lastGoodPos = 0; currentPos != -1; currentPos = input.IndexOfAny(_terminators, currentPos))
                {
                    if (currentPos != -1 && currentPos >= lastBreakPos + lineWidth)
                    {
                        lastBreakPos = lastGoodPos;
                        yield return lastGoodPos + 1; // +1 to put terminator at the end of line, instead of at the beginning of next line
                    }
                    lastGoodPos = currentPos;
                    if (currentPos != -1) currentPos++; // Skips space; otherwise IndexOf() returns the same value over and over again.
                }
            }
        }

        /// <summary>
        /// Adds a defined padding in front of all new lines in the string or StringBuilder.
        /// </summary>
        class PadAllNewLines : ITextProcessor
        {
            protected StringBuilder _builder = new StringBuilder();
            public int PaddingLeft { get { return _paddingLeft; } set { _paddingLeft = value; UpdatePaddingString(); } }
            protected int _paddingLeft = 0;
            public char PaddingChar { get { return _paddingChar; } set { _paddingChar = value; UpdatePaddingString(); } }
            protected char _paddingChar = ' ';
            protected string _paddingString;

            public StringBuilder Process(string input)
            {
                return Process(input, _builder, true);
            }

            public StringBuilder Process(string input, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                if (PaddingLeft == 0) return output.Append(input);

                return AddPaddingToAllNewline(input, output);
            }

            public StringBuilder AddPaddingToAllNewline(string input, StringBuilder output)
            {
                for (int nextNewline = 0, prevNewline = 0; nextNewline != -1;)
                {
                    nextNewline = input.IndexOf(Environment.NewLine, nextNewline);
                    if (nextNewline != -1)
                    {
                        nextNewline += Environment.NewLine.Length; // Don't include newline when we copy
                        output.Append(_paddingString + input.Substring(prevNewline, nextNewline - prevNewline));
                        prevNewline = nextNewline;
                    }
                    // Add rest of string if no more newlines found
                    else output.Append(_paddingString + input.Substring(prevNewline, input.Length - prevNewline));
                }

                return output;
            }

            private void UpdatePaddingString()
            {
                _paddingString = new String(_paddingChar, PaddingLeft);
            }
        }

    }
}
