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
        /// Creates a formatted string representation of position inside a hierarchical tree.
        /// Should receive updated path data through <see cref="OnPathChanged(List{string})"/>.
        /// </summary>
        class Breadcrumb : IControl
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected string _padding = String.Empty;
            protected string _pathSeparator = "›";

            public event Action<StringBuilder> RedrawRequired;

            public Breadcrumb(IBreadcrumbConfig config)
            {
                // TODO: consider whether this needs to be updated when config changes
                _padding = new String(config.PaddingChar, config.PaddingLeft);
                _pathSeparator = config.PathSeparator;
            }

            public StringBuilder GetContent()
            {
                return _buffer;
            }

            public void OnPathChanged(ContentChangeInfo obj)
            {
                var newPath = obj.NavigationPath;

                _buffer.Clear();
                for (int i = 0; i < newPath.Count; i++)
                {
                    _buffer.Append((i == 0 ? _padding : " ") + newPath[i] + (i < newPath.Count - 1 ? " " + _pathSeparator : ""));
                }

                // Doesn't invoke RedrawRequired, because other component does that pertaining to path/folder change.
                // But this should be corrected instead with some aggregation mechanism (to avoid multiple consequent redraws).
            }
        }
    }
}
