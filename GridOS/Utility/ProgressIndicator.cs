using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Simple progress indicator that returns next phase each time you read it.
        /// </summary>
        public class ProgressIndicator
        {
            private int _counter = 0;

            public string Get()
            {
                _counter++;
                switch (_counter % 4)
                {
                    case 0: return ("/");
                    case 1: return ("–");
                    case 2: return ("\\");
                    case 3: return ("|");
                    default: return "";
                }
            }
        }

        public class ProgressIndicator2
        {
            private int _counter = 0;

            public string Get()
            {
                _counter++;
                switch (_counter % 3)
                {
                    case 0: return ("o00");
                    case 1: return ("0o0");
                    case 2: return ("00o");
                    default: return "";
                }
            }
        }
    }
}