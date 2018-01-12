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
        /// Fills and refreshes a TextPanel's "viewport" with content, according to the TextPanel's settings and characteristics.
        /// </summary>
        class DisplayView
        {
            private IMyTextPanel _target;
            private List<string> _content;
            private int? _cursorPosition;
            private int _maxLineWidth;
            public string DisplayHeader { get; set; }
            private ProgressIndicator _spinner = new ProgressIndicator();
            private StringBuilder _builder = new StringBuilder();

            private const string _linePrefixSelected = " > ";
            private const string _linePrefixDefault = "    ";
            private const string _linePrefixMultiline = "     ";

            public DisplayView(IMyTextPanel target)
            {
                _target = target;

                DetermineMaxLineLength();
            }

            private void DetermineMaxLineLength()
            {
                // TODO: Determine max line length based on display type and settings
                _maxLineWidth = 30;
            }
        
            private void UpdateScreen()
            {
                // TODO: Replace static header
                DisplayHeader = $"\r\n    GridOS - Experimental ({_spinner.Get()})\r\n_______________________________________________";

                GetFormattedContent();

                _target.WritePublicText($"{DisplayHeader}{Environment.NewLine}{GetFormattedContent()}");
            }

            private string GetFormattedContent()
            {
                _builder.Clear();

                int i = 0;
                foreach (var s in _content)
                {
                    string prefix;
                    if (i == _cursorPosition)
                        prefix = _linePrefixSelected;
                    else
                        prefix = _linePrefixDefault;

                    if (s.Length > _maxLineWidth)
                    {
                        _builder.AppendLine(WordWrap(s, _maxLineWidth, prefix, _linePrefixMultiline));
                    }
                    else
                    {
                        _builder.AppendLine(prefix + s);
                    }

                    i++;
                }

                return _builder.ToString();
            }


            private void SelectVisibleContent()
            {
                // implement scrolling here, based on current cursor position and screen/font size
            }

            public void Handle_ContentChanged(List<string> content, int? cursorPosition)
            {
                _content = content;
                _cursorPosition = cursorPosition;
                UpdateScreen();
            }

            /// <summary>
            /// Word wraps the given text to fit within the specified width.
            /// </summary>
            /// <param name="text">Text to be word wrapped</param>
            /// <param name="width">Width, in characters, to which the text
            /// should be word wrapped</param>
            /// <returns>The modified text</returns>
            public static string WordWrap(string text, int width, string linePrefixFirst, string linePrefixSubsequent)
            {
                int pos, next;
                StringBuilder sb = new StringBuilder();

                // Lucidity check
                if (width < 1)
                    return text;

                // Parse each line of text
                for (pos = 0; pos < text.Length; pos = next)
                {
                    // Find end of line
                    int eol = text.IndexOf(Environment.NewLine, pos);
                    if (eol == -1)
                        next = eol = text.Length;
                    else
                        next = eol + Environment.NewLine.Length;

                    // Copy this line of text, breaking into smaller lines as needed
                    if (eol > pos)
                    {
                        int linecount = 0;
                        do
                        {
                            linecount++;

                            int len = eol - pos;
                            if (len > width)
                                len = BreakLine(text, pos, width);

                            // Add prefix to lines
                            if (linecount == 1)
                                sb.Append(linePrefixFirst);
                            else
                                sb.Append(linePrefixSubsequent);

                            sb.Append(text, pos, len);
                            sb.Append(Environment.NewLine);

                            // Trim whitespace following break
                            pos += len;
                            while (pos < eol && Char.IsWhiteSpace(text[pos]))
                                pos++;
                        } while (eol > pos);
                    }
                    else sb.Append(Environment.NewLine); // Empty line
                }
                return sb.ToString();
            }

            /// <summary>
            /// Locates position to break the given line so as to avoid
            /// breaking words.
            /// </summary>
            /// <param name="text">String that contains line of text</param>
            /// <param name="pos">Index where line of text starts</param>
            /// <param name="max">Maximum line length</param>
            /// <returns>The modified line length</returns>
            private static int BreakLine(string text, int pos, int max)
            {
                // Find last whitespace in line
                int i = max;
                while (i >= 0 && !Char.IsWhiteSpace(text[pos + i]))
                    i--;

                // If no whitespace found, break at maximum length
                if (i < 0)
                    return max;

                // Find start of whitespace
                while (i >= 0 && Char.IsWhiteSpace(text[pos + i]))
                    i--;

                // Return length of text before whitespace
                return i + 1;
            }
        }
    }
}
