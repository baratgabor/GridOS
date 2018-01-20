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
            private IMyGridProgramRuntimeInfo _runtime;
            private string _targetFont = "Debug";
            private float _targetFontSize = 1.3f;
            private string _formattedContent;
            private string _path = _pathPrefix;
            private int _cursorPosition;
            private int _maxLineWidth;
            private int _maxLineNum = 8;
            private MovementDirection _movementDirection;
            public string DisplayHeader { get; set; }
            private ProgressIndicator2 _spinner = new ProgressIndicator2();
            private StringBuilder _builder = new StringBuilder();

            private const string _linePrefixSelected = " • ";
            private const string _linePrefixDefault = " · ";
            private const string _linePrefixMultiline = "   ";
            private const string _pathPrefix = "  ";
            private const string _pathSeparator = "›";
            private const string _separatorLine = "........................................................................................";
            private const string _separatorLine2 = "˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙˙";

            private int _selectedIndex;
            private List<IDisplayElement> _content;
            public event Action<IDisplayElement> Selected;

            public DisplayView(IMyTextPanel target, IMyGridProgramRuntimeInfo runtime)
            {
                _runtime = runtime;
                _target = target;
                SetupTarget(_target);

                DetermineMaxLineLength();
            }

            private void SetupTarget(IMyTextPanel target)
            {
                target.Font = _targetFont;
                target.FontSize = _targetFontSize;
                target.ShowPublicTextOnScreen();
            }

            private void DetermineMaxLineLength()
            {
                // Proportional to 34 characters at 1.0 font size
                _maxLineWidth = (int)Math.Truncate(34 / _targetFontSize);
            }

            private void UpdateScreen(string formattedString)
            {
                // TODO: Replace static header
                DisplayHeader = $"{Environment.NewLine}  ::GridOS:: {_spinner.Get()} LRT: {_runtime.LastRunTimeMs:G3}ms{Environment.NewLine}{_separatorLine}";

                _target.WritePublicText($"{DisplayHeader}{Environment.NewLine}{_path}{Environment.NewLine}{_separatorLine2}{Environment.NewLine}{formattedString}");
            }

            private string GetFormattedString(List<IDisplayElement> content)
            {
                _builder.Clear();
                for (int i = 0; i < content.Count; i++)
                {
                    string linePrefix;
                    if (i == _cursorPosition)
                        linePrefix = _linePrefixSelected;
                    else
                        linePrefix = _linePrefixDefault;

                    var line = content[i].Label;
                    if (line.Length > _maxLineWidth)
                        _builder.AppendLine(WordWrap(line, _maxLineWidth, linePrefix, _linePrefixMultiline));
                    else
                        _builder.AppendLine(linePrefix + line);
                }

                return _builder.ToString();
            }

            internal void Handle_ContentChanged(List<IDisplayElement> content)
            {
                _content = content;
                _formattedContent = GetFormattedString(_content);
                UpdateScreen(_formattedContent);
            }

            internal void Handle_PathChanged(List<IDisplayGroup> path)
            {
                _builder.Clear();
                _builder.Append(_pathPrefix);
                foreach (var e in path)
                {
                    _builder.Append(" " + e.Label + " " + _pathSeparator);
                }
                _path = _builder.ToString();

                _selectedIndex = 0;
            }

            internal void Handle_SelectionChanged(int cursorPosition)
            {
                _builder.Clear();
                _builder.Append(_formattedContent);

                // Selection moved down
                if (cursorPosition > _cursorPosition)
                {
                    // Remove old selection bullet, replace it with default bullet
                    int selectionBulletIndex = _formattedContent.IndexOf(_linePrefixSelected);
                    _builder.Remove(selectionBulletIndex, _linePrefixSelected.Length);
                    _builder.Insert(selectionBulletIndex, _linePrefixDefault);

                    // Remove next bullet, replace it with selection bullet
                    int nextBulletIndex = _formattedContent.IndexOf(_linePrefixDefault, selectionBulletIndex);
                    _builder.Remove(nextBulletIndex, _linePrefixDefault.Length);
                    _builder.Insert(nextBulletIndex, _linePrefixSelected);
                }

                // Selection moved up
                if (cursorPosition < _cursorPosition)
                {
                    // Remove old selection bullet, replace it with default bullet
                    int selectionBulletIndex = _formattedContent.IndexOf(_linePrefixSelected);
                    _builder.Remove(selectionBulletIndex, _linePrefixSelected.Length);
                    _builder.Insert(selectionBulletIndex, _linePrefixDefault);

                    // Remove next bullet, replace it with selection bullet
                    int prevBulletIndex = _formattedContent.LastIndexOf(_linePrefixDefault, selectionBulletIndex);
                    _builder.Remove(prevBulletIndex, _linePrefixDefault.Length);
                    _builder.Insert(prevBulletIndex, _linePrefixSelected);
                }

                _cursorPosition = cursorPosition;
                _formattedContent = _builder.ToString();
                UpdateScreen(_formattedContent);
            }

            public void MoveUp(CommandItem sender, string param)
            {
                if (_selectedIndex <= 0)
                    return;

                _selectedIndex--;

                Handle_SelectionChanged(_selectedIndex);
            }

            public void MoveDown(CommandItem sender, string param)
            {
                if (_selectedIndex >= _content.Count - 1)
                    return;

                _selectedIndex++;

                Handle_SelectionChanged(_selectedIndex);
            }

            public void Select(CommandItem sender, string param)
            {
                Selected?.Invoke(_content[_selectedIndex]);
            }

            /// <summary>
            /// Word wraps the given text to fit within the specified width.
            /// </summary>
            private string WordWrap(string text, int width, string linePrefixFirst, string linePrefixSubsequent)
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
            private int BreakLine(string text, int pos, int max)
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

            enum MovementDirection
            {
                None = 0,
                Up,
                Down
            }
        }
    }
}
