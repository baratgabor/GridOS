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
        // TODO: Implement LineInfo creation, wire configuration into all components


        interface IControl
        {
            event Action<StringBuilder> RedrawRequired;
            StringBuilder GetContent();
        }

        abstract class Frame : IControl
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IControl _inner;

            public event Action<StringBuilder> RedrawRequired;

            public Frame(IControl inner)
            {
                _inner = inner;
                _inner.RedrawRequired += Process_Redraw;
            }

            public void Get_Process_Redraw()
            {
                Process_Redraw(_inner.GetContent());
            }

            protected void Process_Redraw(StringBuilder input)
            {
                _buffer = Process(input);
                RedrawRequired?.Invoke(_buffer);
            }

            public StringBuilder GetContent()
            {
                return _buffer;
            }

            protected abstract StringBuilder Process(StringBuilder input);
        }

        /// <summary>
        /// Navigation layer on top of a scrollable box
        /// </summary>
        class NavigationFrame : Frame
        {
            protected INavConfig _config;
            protected int _selectedLine = 0;
            protected IScrollable _scrollableBox;

            public NavigationFrame(INavConfig config, IScrollable scrollableBox) : base(scrollableBox)
            {
                _config = config;
                _scrollableBox = scrollableBox;
            }

            public bool MoveUp()
            {
                if (_selectedLine <= 0)
                    return false;

                _selectedLine--;
                AdjustVerticalOffset(_selectedLine);
                return true;
            }

            public bool MoveDown()
            {
                if (_selectedLine >= _scrollableBox.LineNumber - 1)
                    return false;

                _selectedLine++;
                AdjustVerticalOffset(_selectedLine);
                return true;
            }

            protected void AdjustVerticalOffset(int selectedLine)
            {
                int vo = _scrollableBox.VerticalOffset;
                int lh = _scrollableBox.LineHeight;

                if (selectedLine < vo)
                    _scrollableBox.SetVerticalOffset(vo - 1);
                else if (selectedLine + 2 > vo + lh)
                {
                    _scrollableBox.SetVerticalOffset(selectedLine + 2 - lh);
                }
            }

            protected override StringBuilder Process(StringBuilder input)
            {
                _buffer.Clear();
                _buffer.Append(input);
                _buffer[_scrollableBox.LineInfo[_selectedLine].StartPosition + 1] = _config.SelectionMarker;

                return _buffer;
            }
        }

        interface IScrollable : IControl
        {
            int LineNumber { get; }
            int VerticalOffset { get; }
            int LineHeight { get; }
            List<LineInfo> LineInfo { get; }
            bool SetVerticalOffset(int value);
        }

        /// <summary>
        /// Scrollable wrapper around string content
        /// </summary>
        class ScrollableFrame : Frame, IScrollable
        {
            public int VerticalOffset => _verticalOffset;
            public int LineHeight => _config.LineHeight;
            public int LineNumber => _source.LineInfo.Count;
            public List<LineInfo> LineInfo => _source.LineInfo;

            protected int _verticalOffset = 0;
            protected IViewportConfig _config;
            protected ILineInfoProviderControl _source;

            public ScrollableFrame(IViewportConfig config, ILineInfoProviderControl source) : base(source)
            {
                _config = config;
                _source = source;
            }

            public bool SetVerticalOffset(int value)
            {
                if (value == _verticalOffset || value < 0 || value + _config.LineHeight > LineInfo.Count)
                    return false;

                _verticalOffset = value;
                Get_Process_Redraw();
                return true;
            }

            protected override StringBuilder Process(StringBuilder input)
            {
                _buffer.Clear();

                int ViewportStart = LineInfo[_verticalOffset].StartPosition;
                int ViewportEnd = _verticalOffset + _config.LineHeight >= LineInfo.Count ? input.Length : LineInfo[_verticalOffset + _config.LineHeight].StartPosition - 2; // -2 cuts \r\n
                int ViewportLength = ViewportEnd - ViewportStart;

                _buffer.Append(input.ToString(ViewportStart, ViewportLength));
                return _buffer;
            }
        }


        class ProcessingArgs
        {
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public List<LineInfo> LineInfo { get; set; }
            public IDisplayElement Element;
            public int CurrentOutputLength;
        }

        // Yeah, ugly
        interface ILineInfoProviderControl : IControl
        {
            List<LineInfo> LineInfo { get; }
        }

        /// <summary>
        /// Content builder that builds content as a single string from a list of elements
        /// </summary>
        class MenuContentBuilder : ILineInfoProviderControl
        {
            public List<LineInfo> LineInfo => _lineInfo;
            public event Action<StringBuilder> RedrawRequired;

            protected List<IDisplayElement> _menuItems;
            protected List<ITextProcessor> _pipeline = new List<ITextProcessor>();
            protected StringBuilder _buffer = new StringBuilder();
            
            protected IAffixConfig _config;
            protected ProcessingArgs _processingArgs = new ProcessingArgs();
            protected List<LineInfo> _lineInfo = new List<LineInfo>();
            
            public MenuContentBuilder(IAffixConfig config)
            {
                _config = config;
                _processingArgs.LineInfo = _lineInfo;
            }

            public StringBuilder Process()
            {
                _buffer.Clear();
                foreach (var e in _menuItems)
                {
                    _processingArgs.Prefix = _config.GetPrefixFor(e, false);
                    _processingArgs.Suffix = _config.GetSuffixFor(e, false);
                    _processingArgs.Element = e;
                    _processingArgs.CurrentOutputLength = _buffer.Length;

                    string s = e.Label;
                    foreach (var p in _pipeline)
                    {
                        // TODO: Eliminate ToString(); use only StringBuilders
                        s = p.Process(s, _processingArgs).ToString();
                    }
                    _buffer.Append(s + Environment.NewLine);
                }
                _buffer.Remove(_buffer.Length - 2, 2); // Remove last '\r\n' newline
                return _buffer;
            }

            public MenuContentBuilder AddContent(List<IDisplayElement> content)
            {
                _menuItems = content;
                Process_Redraw();
                return this;
            }

            protected void Process_Redraw()
            {
                Process();
                RedrawRequired?.Invoke(_buffer);
            }

            public MenuContentBuilder AddProcessor(ITextProcessor processor)
            {
                _pipeline.Add(processor);
                return this;
            }

            public void ClearProcessors()
            {
                _pipeline.Clear();
            }

            public StringBuilder GetContent()
            {
                return _buffer;
            }
        }

        interface IWordWrappingConfig
        {
            int LineLength { get; }
            char[] Terminators { get; }
        }

        interface IViewportConfig
        {
            int LineHeight { get; }
        }

        interface IPaddingConfig
        {
            char PaddingChar { get; }
            int PaddingLeft { get; }
        }

        interface INavConfig
        {
            char SelectionMarker { get; }
        }

        interface IPrefixConfig
        {
            string Prefix { get; }
            bool AddSpace { get; }
        }

        interface ISuffixConfig
        {
            string Suffix { get; }
            bool AddSpace { get; }
        }

        interface IAffixConfig
        {
            string GetPrefixFor(IDisplayElement element, bool selected);
            string GetSuffixFor(IDisplayElement element, bool selected);
        }

        class SmartConfig : IWordWrappingConfig, IViewportConfig, IPaddingConfig, INavConfig, IAffixConfig
        {
            public int LineLength { get; set; }
            public char[] Terminators { get; set; }

            public int LineHeight { get; set;  }

            public char PaddingChar { get; set; }
            public int PaddingLeft { get; set; }

            public char SelectionMarker { get; set; } = '›';

            public Affix Prefixes_Unselected = new Affix()
            {
                Element = " ",
                Command = "·",
                Group = "·"
            };

            public Affix Suffixes_Unselected = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };
            public Affix Prefixes_Selected = new Affix()
            {
                Element = " ",
                Command = "•",
                Group = "•"
            };
            public Affix Suffixes_Selected = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };

            public string GetPrefixFor(IDisplayElement element, bool selected)
            {
                if (selected) return GetAffix(element, selected, Prefixes_Selected);
                else return GetAffix(element, selected, Prefixes_Unselected);
            }

            public string GetSuffixFor(IDisplayElement element, bool selected)
            {
                if (selected) return GetAffix(element, selected, Suffixes_Selected);
                else return GetAffix(element, selected, Suffixes_Unselected);
            }

            protected string GetAffix(IDisplayElement element, bool selected, Affix affix)
            {
                string value = "";

                if (element is IDisplayGroup)
                    value = affix.Group;
                else if (element is IDisplayCommand)
                    value = affix.Command;
                else if (element is IDisplayElement)
                    value = affix.Element;

                return value;
            }

            public struct Affix
            {
                public string Group;
                public string Command;
                public string Element;
            }
        }

        struct DynamicConfig : IPrefixConfig, ISuffixConfig
        {
            public string Prefix { get; private set; }
            bool IPrefixConfig.AddSpace { get; }
            public string Suffix { get; private set; }
            bool ISuffixConfig.AddSpace { get; }
        }



        interface ITextProcessor
        {
            StringBuilder Process(string input, ProcessingArgs args);
            StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false);
            // Implement this later; needs IndexOfAny implementation for StringBuilder:
            // StringBuilder Process(StringBuilder input, StringBuilder output, bool clearOutput = false);
        }



        class LineInfoExtractor : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IPaddingConfig _config;

            public LineInfoExtractor(IPaddingConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                _buffer.Clear();
                _buffer.Append(input);
                return Process(input, args, _buffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                // TODO: highly assumptious; bulletCharPosition is problematic, since we switch to prefix strings, which can be longer than 1 char.
                // also assumes that structure is "padding + prefix + element", but actually order can be any
                int bulletCharPosition = args.CurrentOutputLength + _config.PaddingLeft;

                for (int currentPos = 0; currentPos != -1; currentPos = input.IndexOf(Environment.NewLine, currentPos))
                {
                    if (currentPos != -1)
                    {
                        if (currentPos != 0) currentPos += Environment.NewLine.Length;
                        args.LineInfo.Add(new LineInfo(currentPos + args.CurrentOutputLength, args.Element, bulletCharPosition));
                        currentPos++;
                    }
                }

                return output;
            }
        }


        /// <summary>
        /// Wraps the provided string or StringBuilder according to the settings given.
        /// </summary>
        class WordWrap_Strategy1 : ITextProcessor
        {
            protected StringBuilder _inputBuffer = new StringBuilder();
            protected StringBuilder _outputBuffer = new StringBuilder();
            protected IWordWrappingConfig _config;

            public WordWrap_Strategy1(IWordWrappingConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _outputBuffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                int prevBreakPos = 0;
                foreach (var breakPos in FindNextLineBreak(input, _config.LineLength))
                {
                    output.Append(input.Substring(prevBreakPos, breakPos - prevBreakPos) + Environment.NewLine);
                    prevBreakPos = breakPos;
                }
                output.Append(input.Substring(prevBreakPos, input.Length - prevBreakPos)); // Rest of string until the end
                return output;
            }

            protected IEnumerable<int> FindNextLineBreak(string input, int lineLength)
            {
                for (int currentPos = 0, lastBreakPos = 0, lastGoodPos = 0; currentPos != -1; currentPos = input.IndexOfAny(_config.Terminators, currentPos))
                {
                    if (currentPos != -1 && currentPos >= lastBreakPos + lineLength)
                    {
                        lastBreakPos = lastGoodPos;
                        yield return lastGoodPos + 1; // +1 to put terminator at the end of line, instead of at the beginning of next line
                    }
                    lastGoodPos = currentPos;
                    if (currentPos != -1) currentPos++; // Skips space; otherwise IndexOf() returns the same value over and over again.
                }
            }
        }

        /// <summary>
        /// Adds a defined padding in front of all new lines in the string or StringBuilder.
        /// </summary>
        class PadAllNewLines : ITextProcessor
        {
            protected StringBuilder _builder = new StringBuilder();
            protected int _paddingLeft;
            protected char _paddingChar;
            protected string _paddingString;
            protected IPaddingConfig _config;

            public PadAllNewLines(IPaddingConfig config)
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

                if (_config.PaddingChar != _paddingChar || _config.PaddingLeft != _paddingLeft)
                    UpdatePaddingString();

                return AddPaddingToAllNewline(input, output);
            }

            protected StringBuilder AddPaddingToAllNewline(string input, StringBuilder output)
            {
                for (int nextNewline = 0, prevNewline = 0; nextNewline != -1;)
                {
                    nextNewline = input.IndexOf(Environment.NewLine, nextNewline);
                    if (nextNewline != -1)
                    {
                        nextNewline += Environment.NewLine.Length; // Don't include newline when we copy
                        output.Append(_paddingString + input.Substring(prevNewline, nextNewline - prevNewline));
                        prevNewline = nextNewline;
                    }
                    // Add rest of string if no more newlines found
                    else output.Append(_paddingString + input.Substring(prevNewline, input.Length - prevNewline));
                }

                return output;
            }

            private void UpdatePaddingString()
            {
                _paddingChar = _config.PaddingChar;
                _paddingLeft = _config.PaddingLeft;
                _paddingString = new String(_paddingChar, _paddingLeft);
            }
        }

        class AddPrefix : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected IPrefixConfig _config;

            public AddPrefix(IPrefixConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _buffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                output.Append(_config.Prefix + (_config.AddSpace ? " " : "") + input);
                return output;
            }
        }

        class AddSuffix : ITextProcessor
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected ISuffixConfig _config;

            public AddSuffix(ISuffixConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingArgs args)
            {
                return Process(input, args, _buffer, true);
            }

            public StringBuilder Process(string input, ProcessingArgs args, StringBuilder output, bool clearOutput = false)
            {
                if (clearOutput == true)
                    output.Clear();

                output.Append(input + (_config.AddSpace ? " " : "") + _config.Suffix);
                return output;
            }
        }
    }
}
