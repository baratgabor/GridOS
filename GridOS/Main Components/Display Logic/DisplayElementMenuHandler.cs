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
        class DisplayElementMenuHandler
        {
            private StringBuilder _builder_Temp = new StringBuilder(); // Generic reusable builder

            private Flush _flush = Flush.All; // Cache flushing controller flags
            private List<IDisplayElement> _elements; // Raw content to be abstracted
            private StringBuilder _builder_Formatted = new StringBuilder(); // 1st (caching) stage of content abstraction
            private StringBuilder _builder_FormattedView = new StringBuilder(); // 2nd (caching) stage of content abstraction
            private List<LineInfo> _lineInfo = new List<LineInfo>(); // Text line data for view filtering and translation from primary selection (line) to secondary (element)

            private LoopingPipeline<Command_ProcessDisplayElement, StringBuilder> _displayElementProcessor;
            private Command_ProcessDisplayElement _displayElementProcessingCommand = new Command_ProcessDisplayElement();

            private Dictionary<Type, char> _prefixMap = new Dictionary<Type, char>() {
                { typeof(IDisplayCommand), '·' },
                { typeof(IDisplayGroup), '·' },
                { typeof(IDisplayElement), ' ' }
            };
            private Dictionary<Type, char> _suffixMap = new Dictionary<Type, char>() {
                { typeof(IDisplayCommand), ' ' },
                { typeof(IDisplayGroup), '»' },
                { typeof(IDisplayElement), ' ' }
            };

            private Dictionary<ValidationRule, Func<int, bool>> _intValidationMap = new Dictionary<ValidationRule, Func<int, bool>>() { // Predicate not supported in SE
                { ValidationRule.None, (x) => true },
                { ValidationRule.ForceZeroOrPositive, (x) => x >= 0 },
                { ValidationRule.ForceGreaterThanZero, (x) => x > 0 }
            };

            // Public properties with flush controlling setters
            public int MaxWidth { get { return _maxWidth; } set { TrySet(ref _maxWidth, value, ValidationRule.ForceZeroOrPositive, Flush.All); } }
            private int _maxWidth = 0;
            public int LineHeight { get { return _lineHeight; } set { TrySet(ref _lineHeight, value, ValidationRule.ForceZeroOrPositive, Flush.View); } }
            private int _lineHeight = 0;
            public int VerticalOffset { get { return _verticalOffset; } set { TrySet(ref _verticalOffset, value, ValidationRule.ForceZeroOrPositive, Flush.View); } }
            private int _verticalOffset = 0;
            public int LeftPadding { get { return _leftPadding; } set { TrySet(ref _leftPadding, value, ValidationRule.ForceZeroOrPositive, Flush.All); } }
            private int _leftPadding = 0;

            // TODO: implement WordWrap switch - strategy
            public bool WordWrap { get; set; } = false;

            public event Action RedrawRequired;

            private const char _itemBulletDefault = '·';
            private const char _itemBulletSelected = '•';
            private const char _selectedLineBullet = '›';
            private const char _groupItemSuffix = '»';
            private readonly string _newLine = Environment.NewLine;

            private int _selectedLine = 0; // Primary selection tracking
            public IDisplayElement SelectedElement => _selectedElement;
            private IDisplayElement _selectedElement; // Secondary selection tracking (derived from _lineInfo[_selectedLine])

            public DisplayElementMenuHandler()
            {

                /*
                _displayElementProcessor =
                    new CachingDecorator(
                        new LoopingTransformingPipeline<Command_ProcessDisplayElement, string, StringBuilder>());
                //new TransformStringToStringBuilder();
                */


                // TODO: This refactor failed. Change to cleaner separation of concerns by first running pure word-wrap per displayelement/string, then separate additional processing. Employ caching to offset overhead.
                _displayElementProcessor =
                        //  new CachingDecorator(
                        new LoopingPipeline<Command_ProcessDisplayElement, StringBuilder>();
                        //);

                _displayElementProcessor
                    .Add(new Processor_AddNewLine())
                    .Add(new Processor_AddLineInfo(_lineInfo, _leftPadding))
                    .Add(new Processor_AddLinePrefix(_leftPadding))
                    .Add(new Processor_AddLineContent(_maxWidth));
            }

            private void TrySet<T>(ref T field, T value, Flush flush = Flush.None)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                    return;

                _flush |= flush;
                field = value;
            }

            // Boxes ints, but not hot code, so shouldn't cause performance issues
            private void TrySet(ref int field, int value, ValidationRule rule = ValidationRule.None, Flush flush = Flush.None)
            {
                if (field == value)
                    return;

                if (!_intValidationMap[rule].Invoke(value))
                {
                    throw new Exception($"Argument found invalid by validation rule: {rule.ToString()}");
                }

                _flush |= flush;
                field = value;
            }

            /// <summary>
            /// Replace all content and reset state.
            /// </summary>
            /// <param name="elements">Elements to set.</param>
            public void SetMenuElements(List<IDisplayElement> elements)
            {
                _elements = elements;
                _selectedLine = 0;
                _selectedElement = null;
                _verticalOffset = 0;
                _flush = Flush.All;
            }

            public void SetSelectedElement(IDisplayElement element)
            {
                if (element == null)
                    return;

                _selectedElement = element;

                // TODO: we probably shouldn't call buildformat directly from here
                BuildFormat();
                _flush = Flush.View;
                AdjustViewport(_selectedLine);
            }

            // TODO: Test and make it nicer
            public void UpdateElement(IDisplayElement element)
            {
                // TODO: Refactor ineffecient queries
                LineInfo li = _lineInfo.FirstOrDefault((x) => x.ParentDisplayElement == element);

                if (li.Equals(default(LineInfo)))
                    return;

                int i = _lineInfo.IndexOf(li);

                int endPos;
                if (_lineInfo.Count < i + 1)
                    endPos = _builder_Formatted.Length;
                else
                    endPos = _lineInfo[i + 1].StartPosition - _newLine.Length;

                // TODO: Efficiently store/retrieve old string to replace
                string oldStr = _builder_Formatted.ToString(li.StartPosition, endPos - li.StartPosition);

                _builder_Temp.Clear();
                string newStr = _displayElementProcessor.Process(SetupCommand(element), _builder_Temp).ToString();

                // None of the StringBuilder.Replace overloads seem ideal
                _builder_Formatted.Replace(oldStr, newStr, li.StartPosition, endPos);
                _flush = Flush.View;
            }

            public void MoveUp()
            {
                if (_selectedLine <= 0)
                    return;

                // TODO: Consider adding error checking to see if _selectedLine is not out of bounds for _lines array

                UpdateSelection(_selectedLine - 1);
            }

            public void MoveDown()
            {
                if ((_flush & Flush.All) != 0)
                {
                    BuildFormat(); // Build the _lines data necessary for safe selection increment
                    _flush &= ~Flush.All;
                }

                if (_selectedLine >= _lineInfo.Count - 1)
                    return;

                UpdateSelection(_selectedLine + 1);
            }

            private void UpdateSelection(int newSelectedLine)
            {
                // TODO: refactor these bullet changing hacks
                if (_builder_Formatted[_lineInfo[_selectedLine].StartPosition + 1] == _selectedLineBullet)
                    _builder_Formatted[_lineInfo[_selectedLine].StartPosition + 1] = ' ';
                if (_builder_Formatted[_lineInfo[newSelectedLine].StartPosition + 1] == ' ')
                    _builder_Formatted[_lineInfo[newSelectedLine].StartPosition + 1] = _selectedLineBullet;

                // If selected display element needs to be changed
                if (_selectedElement != null && _lineInfo[newSelectedLine].ParentDisplayElement != _selectedElement)
                {
                    // Remove selection bullet from previously selected element
                    _builder_Formatted[_lineInfo[_selectedLine].BulletCharPosition] = _itemBulletDefault;
                    // Add selection bullet to newly selected element
                    _builder_Formatted[_lineInfo[newSelectedLine].BulletCharPosition] = _itemBulletSelected;
                }

                _selectedLine = newSelectedLine;
                _selectedElement = _lineInfo[_selectedLine].ParentDisplayElement;
                AdjustViewport(_selectedLine);
                _flush |= Flush.View;
                RedrawRequired?.Invoke();
            }

            private void AdjustViewport(int selectedLine)
            {
                if (selectedLine < _verticalOffset)
                    _verticalOffset--;
                if (selectedLine + 2 > _verticalOffset + _lineHeight)
                {
                    _verticalOffset = selectedLine + 2 - _lineHeight;
                }
            }

            public string GetContent()
            {
                switch (_flush)
                {
                    case Flush.None:
                        break;
                    case Flush.All:
                        BuildFormat();
                        BuildView();
                        break;
                    case Flush.All | Flush.View:
                        BuildFormat();
                        BuildView();
                        break;
                    case Flush.View:
                        BuildView();
                        break;
                }

                _flush = Flush.None;

                return _builder_FormattedView.ToString();
            }

            public void FlushCaches()
            {
                _flush = Flush.All;
            }

            private void BuildView()
            {
                _builder_FormattedView.Clear();

                int ViewportStart = _lineInfo[_verticalOffset].StartPosition;
                int ViewportEnd = _verticalOffset + _lineHeight >= _lineInfo.Count ? _builder_Formatted.Length : _lineInfo[_verticalOffset + _lineHeight].StartPosition - 2; // -2 cuts \r\n

                _builder_FormattedView.Append(_builder_Formatted.ToString(ViewportStart, ViewportEnd - ViewportStart));

                /* // Seems markedly slower than simply creating a string... test it further
                for (int i = ViewportStart; i < ViewportEnd; i++)
                    _builder_FormattedView.Append(_builder_Formatted[i]);
                */
            }

            private void BuildFormat()
            {
                _builder_Formatted.Clear();
                _lineInfo.Clear();

                foreach (var element in _elements)
                {
                    _displayElementProcessingCommand = SetupCommand(element);
                    _displayElementProcessor.Process(_displayElementProcessingCommand, _builder_Formatted);
                }
            }

            private Command_ProcessDisplayElement SetupCommand(IDisplayElement element)
            {
                _displayElementProcessingCommand.DisplayElement = element;

                if (element.GetType() is IDisplayElement)
                    _displayElementProcessingCommand.Bullet = _prefixMap[typeof(IDisplayElement)];
                else if (element.GetType() is IDisplayGroup)
                    _displayElementProcessingCommand.Bullet = _prefixMap[typeof(IDisplayGroup)];
                else if (element.GetType() is IDisplayCommand)
                    _displayElementProcessingCommand.Bullet = _prefixMap[typeof(IDisplayCommand)];

                _displayElementProcessingCommand.CurrentLineWidth = 0;
                _displayElementProcessingCommand.InputProgress = 0;
                _displayElementProcessingCommand.Loop = true;
                _displayElementProcessingCommand.Selected = _selectedElement == element ? true : false;

                return _displayElementProcessingCommand;
            }

            [Flags]
            private enum Flush
            {
                None = 0,
                Prefixes = 1,
                View = 2,
                All = 4
            }

            private enum ValidationRule
            {
                None,
                ForceZeroOrPositive,
                ForceGreaterThanZero
            }
        }

        struct LineInfo
        {
            public readonly int StartPosition;
            public readonly IDisplayElement ParentDisplayElement;
            public readonly int BulletCharPosition;

            public LineInfo(int startPosition, IDisplayElement parentDisplayElement, int bulletCharPosition)
            {
                StartPosition = startPosition;
                ParentDisplayElement = parentDisplayElement;
                BulletCharPosition = bulletCharPosition;
            }
        }


        class CachingDecorator : LoopingPipeline<Command_ProcessDisplayElement, StringBuilder>
        {
            protected Dictionary<IDisplayElement, string> _cache = new Dictionary<IDisplayElement, string>();
            protected LoopingPipeline<Command_ProcessDisplayElement, StringBuilder> _original;
            protected StringBuilder _builder = new StringBuilder();

            public CachingDecorator(LoopingPipeline<Command_ProcessDisplayElement, StringBuilder> pipeline)
            {
                _original = pipeline;
            }

            public override StringBuilder Process(Command_ProcessDisplayElement input, StringBuilder output)
            {
                string value = _cache.GetValueOrDefault(input.DisplayElement);

                // If not cached, get fresh and cache it
                if (value.Equals(default(string)))
                {
                    _builder.Clear();
                    value = _original.Process(input, _builder).ToString();
                    _cache.Add(input.DisplayElement, value);
                    input.DisplayElement.LabelChanged += RemoveElement;
                }

                output.Append(value);
                return output;
            }

            public override IProcessorComposite<Command_ProcessDisplayElement, StringBuilder> Add(IProcessor<Command_ProcessDisplayElement, StringBuilder> processor)
            {
                return _original.Add(processor);
            }

            private void RemoveElement(IDisplayElement element)
            {
                element.LabelChanged -= RemoveElement;
                _cache.Remove(element);
            }

            private void RemoveAll()
            {
                foreach (var item in _cache)
                {
                    item.Key.LabelChanged -= RemoveElement;
                }
                _cache.Clear();
            }
        }

        class Command_ProcessDisplayElement : ILoopSignal
        {
            public IDisplayElement DisplayElement;
            public int InputProgress;
            public int CurrentLineWidth;
            public bool Selected;
            public char Bullet;
            public bool Loop { get; set; } = false;
        }

        class Processor_AddLineInfo : IProcessor<Command_ProcessDisplayElement, StringBuilder>
        {
            private List<LineInfo> _lineInfo;
            private int _leftPadding;

            public Processor_AddLineInfo(List<LineInfo> lineInfo, int leftPadding)
            {
                _lineInfo = lineInfo;
                _leftPadding = leftPadding;
            }

            public StringBuilder Process(Command_ProcessDisplayElement input, StringBuilder output)
            {
                _lineInfo.Add(new LineInfo(
                    startPosition: output.Length,
                    parentDisplayElement: input.DisplayElement,
                    bulletCharPosition: output.Length + _leftPadding
                    ));

                return output;
            }
        }

        class Processor_AddNewLine : IProcessor<Command_ProcessDisplayElement, StringBuilder>
        {
            public StringBuilder Process(Command_ProcessDisplayElement input, StringBuilder output)
            {
                if (output.Length > 0) // don't add newline before first line
                    output.Append(Environment.NewLine);

                input.CurrentLineWidth = 0;
                return output;
            }
        }

        class Processor_AddLinePrefix : IProcessor<Command_ProcessDisplayElement, StringBuilder>
        {
            private int _leftPadding;

            public Processor_AddLinePrefix(int leftPadding)
            {
                _leftPadding = leftPadding;
            }

            public StringBuilder Process(Command_ProcessDisplayElement input, StringBuilder output)
            {
                string _linePrefix = new string(' ', _leftPadding) + input.Bullet + ' ';
                output.Append(_linePrefix);
                input.CurrentLineWidth += _linePrefix.Length;
                return output;
            }
        }

        class Processor_AddLineContent : IProcessor<Command_ProcessDisplayElement, StringBuilder>
        {
            private int _maxLineWidth;

            public Processor_AddLineContent(int lineWidth)
            {
                _maxLineWidth = lineWidth;
            }

            public StringBuilder Process(Command_ProcessDisplayElement input, StringBuilder output)
            {
                string str = input.DisplayElement.Label;
                int NextBreakPoint = FindNextBreakPoint(str, input.InputProgress, _maxLineWidth - input.CurrentLineWidth);
                int NextLineLength = NextBreakPoint - input.InputProgress;
                string newLine = str.Substring(input.InputProgress, NextLineLength);
                output.Append(newLine);
                input.CurrentLineWidth += newLine.Length;
                input.InputProgress = NextBreakPoint;

                if (NextBreakPoint == str.Length)
                    input.Loop = false;
                else
                    input.Loop = true;

                return output;
            }

            private int FindNextBreakPoint(string input, int startingPos, int maxWidth)
            {
                int breakpoint = startingPos;

                while (breakpoint < startingPos + maxWidth)
                {
                    breakpoint = input.IndexOf(' ', breakpoint);

                    if (breakpoint == -1)
                    {
                        breakpoint = input.Length;
                        break;
                    }
                }

                return breakpoint;
            }
        }

    }
}