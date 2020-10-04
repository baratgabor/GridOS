﻿using System.Text;
using System;

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
            protected IMenuPresentationConfig _config;

            protected string _paddingString_FirstLine;

            public PadAllLines(IMenuPresentationConfig config)
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

                _paddingString = new string(_paddingChar, _paddingLeft + 2); // To account for fixed length Prefix on the 1st line of menu items. Leaky abstraction, but this processing pathway will be obsoleted soon anyway.
                _paddingString_FirstLine = new string(_paddingChar, _paddingLeft);
            }

            protected bool PaddingChanged()
                => (_config.PaddingChar != _paddingChar || _config.PaddingLeft != _paddingLeft);
        }
    }
}