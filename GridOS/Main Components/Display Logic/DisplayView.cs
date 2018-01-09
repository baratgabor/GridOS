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
            private string _content;
            private int? _cursorPosition;
            public string DisplayHeader { get; set; }
            private ProgressIndicator _spinner = new ProgressIndicator();

            public DisplayView(IMyTextPanel target)
            {
                _target = target;
            }

            private void UpdateScreen()
            {
                // TODO: Replace static header
                DisplayHeader = $"\r\n    GridOS - Experimental ({_spinner.Get()})\r\n_______________________________________________";

                _target.WritePublicText($"{DisplayHeader}{Environment.NewLine}{_content}");
            }

            private void SelectVisibleContent()
            {
                // implement scrolling here, based on current cursor position and screen/font size
            }

            public void Handle_ContentChanged(string content, int? cursorPosition)
            {
                _content = content;
                _cursorPosition = cursorPosition;
                UpdateScreen();
            }
        }
    }
}
