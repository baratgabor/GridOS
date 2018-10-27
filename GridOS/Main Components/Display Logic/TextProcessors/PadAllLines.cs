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
            protected int _paddingLeft_FirstLine;
            protected char _paddingChar;
            protected string _paddingString;
            protected IPaddingConfig _config;

            protected string _paddingString_FirstLine;

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

                if (PaddingChanged())
                    UpdatePaddingString();

                return AddPaddingToAllNewline(input, output);
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                Process(inputOutput.ToString(), args, inputOutput, true);
            }

            protected StringBuilder AddPaddingToAllNewline(string input, StringBuilder output)
            {
                string paddingString = "";

                for (int nextNewline = 0, prevNewline = 0; nextNewline != -1;)
                {
                    nextNewline = input.IndexOf(Environment.NewLine, nextNewline);
                    if (nextNewline != -1)
                    {
                        if (prevNewline == 0)
                            paddingString = _paddingString_FirstLine;
                        else
                            paddingString = _paddingString;

                        nextNewline += Environment.NewLine.Length; // Don't include newline when we copy
                        output.Append(paddingString + input.Substring(prevNewline, nextNewline - prevNewline));
                        prevNewline = nextNewline;
                    }
                    // Add rest of string if no more newlines found
                    else
                    {
                        if (prevNewline == 0)
                            paddingString = _paddingString_FirstLine;
                        else
                            paddingString = _paddingString;

                        output.Append(paddingString + input.Substring(prevNewline, input.Length - prevNewline));
                    }
                }

                return output;
            }

            protected void UpdatePaddingString()
            {
                _paddingChar = _config.PaddingChar;
                _paddingLeft = _config.PaddingLeft;
                _paddingLeft_FirstLine = _config.PaddingLeft_FirstLine;

                _paddingString = new string(_paddingChar, _paddingLeft);
                _paddingString_FirstLine = new string(_paddingChar, _paddingLeft_FirstLine);
            }

            protected bool PaddingChanged()
                => (_config.PaddingChar != _paddingChar
                 || _config.PaddingLeft != _paddingLeft
                 || _config.PaddingLeft_FirstLine != _paddingLeft_FirstLine);
        }
    }
}
