using System.Text;
using System;

namespace IngameScript
{
    partial class Program
	{
        /// <summary>
        /// Wraps the provided string or StringBuilder according to the settings given.
        /// Break-presearch strategy: proactively searches for first breakable character, copies substrings instead of char array iteration.
        /// </summary>
        class WordWrap_NonAllocating : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IMenuPresentationConfig _config;

            public WordWrap_NonAllocating(IMenuPresentationConfig config)
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

            public StringBuilder Process(StringBuilder input, ProcessingArgs _, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                string inputString = input.ToString(); // This does in fact allocate, but this processing pathway will be replaced soon anyway.
                int maxLineLength = _config.LineLength;

                foreach (var line in StringHelpers.WordWrap(inputString, maxLineLength, _config.WordDelimiters))
                {
                    output.Append(inputString, line.Start, line.Length);
                    output.Append(Environment.NewLine);
                }

                output.Length -= Environment.NewLine.Length; // Trim trailing newline.

                return output;
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
