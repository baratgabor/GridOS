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
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Adds a defined padding in front of all new lines in the string or StringBuilder.
        /// </summary>
        class PadAllLines : IMenuItemProcessor
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

            public void Process(StringBuilder processable, IMenuItem referenceMenuItem)
            {
                // If padding not requested, no change needed
                if (_config.PaddingLeft == 0)
                    return;

                // If padding settings changed since last run, build new padding string
                if (PaddingChanged())
                    UpdatePaddingString();

                // Send string copy as input, and cleared StringBuilder as output
                AddPaddingToAllNewline(processable.ToString(), processable.Clear());
            }

            protected StringBuilder AddPaddingToAllNewline(string input, StringBuilder output)
            {
                string paddingString = String.Empty;

                for (int currentBreakPoint = 0, previousBreakPoint = 0; currentBreakPoint != -1;)
                {
                    currentBreakPoint = input.IndexOf(Environment.NewLine, currentBreakPoint);

                    // If first iteration, set special first line padding string
                    if (previousBreakPoint == 0)
                        paddingString = _paddingString_FirstLine;

                    // If no more breakpoints are found, set position to end of string to copy the rest
                    if (currentBreakPoint == -1)
                        currentBreakPoint = input.Length;
                    // If breakpoint is found, adjust position to include the detected newline when copying
                    else
                        currentBreakPoint += Environment.NewLine.Length;

                    output.Append(paddingString);
                    output.Append(input, previousBreakPoint, currentBreakPoint - previousBreakPoint); // Add current line content
                    previousBreakPoint = currentBreakPoint;

                    // Set padding string for the rest of iterations
                    // at the end of the first iteration
                    if (previousBreakPoint == 0)
                        paddingString = _paddingString;
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
