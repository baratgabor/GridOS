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

            //private LoopingPipeline<Command_ProcessDisplayElement, StringBuilder> _displayElementProcessor;
            //private Command_ProcessDisplayElement _displayElementProcessingCommand = new Command_ProcessDisplayElement();

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

                // proto
                var config = new SmartConfig();
                var dynconfig = new DynamicConfig(); // will be created dynamically, need to wire it in
                var builder = new MenuContentBuilder(config);

                builder
                    .AddProcessor(new WordWrap_Strategy1(config))
                    .AddProcessor(new AddPrefix(dynconfig))
                    .AddProcessor(new AddSuffix(dynconfig))
                    .AddProcessor(new PadAllNewLines(config))
                    .AddContent(_elements);

                var root = new NavigationFrame(config,
                    new ScrollableFrame(config,
                        builder));

                // Wire in root redraw into View (but not from this class; this class will likely be obsolete):
                // root.RedrawRequired += 
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
                //string newStr = _displayElementProcessor.Process(SetupCommand(element), _builder_Temp).ToString();

                // None of the StringBuilder.Replace overloads seem ideal
                //_builder_Formatted.Replace(oldStr, newStr, li.StartPosition, endPos);
                _flush = Flush.View;
            }

            private void UpdateSelection(int newSelectedLine)
            {
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

            }

            private void BuildFormat()
            {
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


    }
}