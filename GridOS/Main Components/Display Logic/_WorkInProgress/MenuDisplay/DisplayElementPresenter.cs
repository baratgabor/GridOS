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
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        class DisplayElementPresenter
        {
            public readonly IDisplayElement OriginalElement;
            public string PresentableContent { get; protected set; }
            public List<StringSegment> Lines { get; protected set; } = new List<StringSegment>();

            public DisplayElementPresenter(IDisplayElement element, string content)
            {
                OriginalElement = element;
                PresentableContent = content;
                CreateLineInfo(content);
            }

            private void CreateLineInfo(string content)
            {
                // TODO: Implement LineInfo creation, or rather, move that logic out and keep this class as a DTO
            }
        }        
    }
}
