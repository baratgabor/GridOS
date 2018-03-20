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
            private Flush _flush = Flush.All; // Cache flushing controller flags
            private List<IDisplayElement> _elements; // Raw content to be abstracted
            
            /*
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
            */

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

            public event Action RedrawRequired;


            public DisplayElementMenuHandler()
            {

                // proto
                var config = new SmartConfig();
                var builder = new MenuContentBuilder(config);

                builder
                    .AddProcessor(new WordWrap_BreakPresearchStrategy(config))
                    .AddProcessor(new AddPrefix())
                    .AddProcessor(new AddSuffix())
                    .AddProcessor(new PadAllLines(config))
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

            }

            public void SetSelectedElement(IDisplayElement element)
            {
            }

            // TODO: Test and make it nicer
            public void UpdateElement(IDisplayElement element)
            {

            }

            private void UpdateSelection(int newSelectedLine)
            {
            }

            public string GetContent()
            {
                /*
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
                */

                return "";
            }

            public void FlushCaches()
            {
                _flush = Flush.All;
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