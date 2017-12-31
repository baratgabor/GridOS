using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Quick and dirty progress indicator that changes to next phase each time you fetch it, creating "moving" effect.
        /// </summary>
        public class ProgressIndicator
        {
            private int _progressIndicatorCounter = 0;

            public string Get()
            {
                if (_progressIndicatorCounter == 0)
                {
                    _progressIndicatorCounter = 1;
                    return "-";
                }
                if (_progressIndicatorCounter == 1)
                {
                    _progressIndicatorCounter = 2;
                    return "\\";
                }
                if (_progressIndicatorCounter == 2)
                {
                    _progressIndicatorCounter = 0;
                    return "/";
                }
                else
                {
                    return "";
                }
            }
        }
    }
}