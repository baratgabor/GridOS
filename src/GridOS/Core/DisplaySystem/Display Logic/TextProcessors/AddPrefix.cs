using System.Text;

namespace IngameScript
{
	partial class Program
	{
        /// <summary>
        /// Simply adds a prefix to the input.
        /// </summary>
        class AddPrefix : ITextProcessor
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

                output
                    .Append(args.Prefix)
                    .Append(' ')
                    .Append(input);

                return output;
            }

            public void Process(StringBuilder inputOutput, ProcessingArgs args)
            {
                inputOutput.Insert(0, ' ');
                inputOutput.Insert(0, args.Prefix);
            }
        }
    }
}
