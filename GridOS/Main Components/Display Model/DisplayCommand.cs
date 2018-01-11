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
        /// Specialized node type that holds a delegate to a method, used for executing actions.
        /// </summary>
        class DisplayCommand : DisplayElement, IDisplayCommand
        {
            public event Action<IDisplayCommand> Executed;
            public event Action<IDisplayCommand> BeforeExecute;

            protected Action _command;

            public DisplayCommand(string label, Action action) : base(label)
            {
                _command = action;
            }

            public void Execute()
            {
                BeforeExecute?.Invoke(this);
                _command?.Invoke();
                Executed?.Invoke(this);
            }
        }
    }
}
