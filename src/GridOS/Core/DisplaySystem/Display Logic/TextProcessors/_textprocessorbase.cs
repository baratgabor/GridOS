using System.Text;

namespace IngameScript
{
	partial class Program
	{
        interface ITextProcessor
        {
            StringBuilder Process(string input, ProcessingArgs args);
            StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false);

            void Process(StringBuilder inputOutput, ProcessingArgs args);
            //void Process(StringBuilder input, ProcessingArgs args, StringBuilder output, bool clearOutput = false);
        }
    }
}
