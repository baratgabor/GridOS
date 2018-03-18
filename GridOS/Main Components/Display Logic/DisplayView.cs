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
            private string _path = _pathPrefix;
            private int _maxLineWidth;
            private int _maxLineNum = 8;
            public string DisplayHeader { get; set; }
            private ProgressIndicator2 _spinner = new ProgressIndicator2();
            private StringBuilder _builder = new StringBuilder();

            private const string _pathPrefix = "  ";
            private const string _pathSeparator = "›";
            private const char _lineSeparatorCharTop = '.';
            private const char _lineSeparatorCharBottom = '˙';
            private string _separatorLineTop;
            private string _separatorLineBottom;

            public event Action<IDisplayElement> Selected;

            public DisplayView(IMyTextPanel target, IMyGridProgramRuntimeInfo runtime)
            {
                _runtime = runtime;
                _target = target;

                SetupTarget(_target);
                _maxLineWidth = DetermineMaxLineLength();
                _separatorLineTop = new String(_lineSeparatorCharTop, _maxLineWidth * 2);
                _separatorLineBottom = new String(_lineSeparatorCharBottom, _maxLineWidth * 2);


            }

            private void SetupTarget(IMyTextPanel target)
            {
                target.Font = _targetFont;
                target.FontSize = _targetFontSize;
                target.ShowPublicTextOnScreen();
            }

            private int DetermineMaxLineLength()
            {
                // Proportional to 42 characters at 1.0 font size
                return (int)Math.Truncate(42 / _targetFontSize);
            }

            private void UpdateScreen(string formattedString)
            {
                DisplayHeader = $"{Environment.NewLine}  ::GridOS:: {_spinner.Get()} LRT: {_runtime.LastRunTimeMs:G3}ms{Environment.NewLine}{_separatorLineTop}";

                _target.WritePublicText($"{DisplayHeader}{Environment.NewLine}{_path}{Environment.NewLine}{_separatorLineBottom}{Environment.NewLine}{formattedString}");
            }

            internal void Handle_ContextChanged(ContentChangeInfo changeInfo)
            {

            }

            internal void UpdatePathString(List<IDisplayGroup> path)
            {
                _builder.Clear();
                _builder.Append(_pathPrefix);
                foreach (var group in path)
                {
                    _builder.Append(" " + group.Label + " " + _pathSeparator);
                }
                _path = _builder.ToString();
            }

            internal void Redraw()
            {

            }

            public void Handle_ElementChanged(IDisplayElement element)
            {
            }

            internal void Handle_ContentChanged(List<IDisplayElement> elements)
            {
            }

            public void MoveUp(CommandItem sender, string param)
            {
                //_displayElementMenu.MoveUp();
            }

            public void MoveDown(CommandItem sender, string param)
            {
                //_displayElementMenu.MoveDown();
            }

            public void Select(CommandItem sender, string param)
            {
                //Selected?.Invoke(_displayElementMenu.SelectedElement);
            }
        }
    }
}
