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
        /// Adds a defined padding in front of all new lines in the string or StringBuilder.
        /// </summary>
        class PadAllLines : ITextProcessor
        {
            protected StringBuilder _builder = new StringBuilder();
            protected int _paddingLeft;
            protected char _paddingChar;
            protected string _paddingString;
            protected IPaddingConfig _config;

            public PadAllLines(IPaddingConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _builder, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                if (_config.PaddingLeft == 0) return output.Append(input);

                if (_config.PaddingChar != _paddingChar || _config.PaddingLeft != _paddingLeft)
                    UpdatePaddingString();

                return AddPaddingToAllNewline(input, output);
            }

            protected StringBuilder AddPaddingToAllNewline(string input, StringBuilder output)
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
                _paddingChar = _config.PaddingChar;
                _paddingLeft = _config.PaddingLeft;
                _paddingString = new String(_paddingChar, _paddingLeft);
            }
        }
    }
}
