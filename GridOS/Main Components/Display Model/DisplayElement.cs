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
        /// Basic node in composite pattern based tree structure, used for displaying non-interactive text content.
        /// Serves as a base class for other node types.
        /// </summary>
        public class DisplayElement : IDisplayElement
        {
            public string Label
            {
                get { return _label; }
                set { SetAndNotify(ref _label, value); }
            }
            protected string _label;

            public event Action<IDisplayElement> LabelChanged;

            public DisplayElement(string label)
            {
                _label = label;
            }

            protected void SetAndNotify<T>(ref T field, T value)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                    return;

                field = value;
                LabelChanged?.Invoke(this);
                return;
            }
        }
    }
}
