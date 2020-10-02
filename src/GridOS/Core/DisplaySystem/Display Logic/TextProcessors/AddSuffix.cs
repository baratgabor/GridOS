using System.Text;

namespace IngameScript
{
	partial class Program
	{
        /// <summary>
        /// Simply adds a suffix to the input.
        /// </summary>
        class AddSuffix : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _buffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                output.Append(input);
                
                if (args.Suffix != ' ')
                {
                    output
                        .Append(' ')
                        .Append(args.Suffix);
                }

                return output;
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                inputOutput.Append(' ');
                inputOutput.Append(args.Suffix);
            }
        }
    }
}
