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
        // TODO: Structure properly, clean up, expand

        /// <summary>
        /// Built-in menu nodes for GridOS
        /// </summary>
        class HelpMenu : MenuGroup
        {
            private MenuItem _helpTextGeneric = new MenuItem($"Welcome to GridOS!{Environment.NewLine}{Environment.NewLine}GridOS is a modular multitasking system with flexible menu display capabilities.{Environment.NewLine}{Environment.NewLine}The following subpages will help you to understand the usage of GridOS:{Environment.NewLine}");

            private MenuGroup _moreInfo_Modules = new MenuGroup("Modules");
            private MenuGroup _moreInfo_AutoUpdates = new MenuGroup("Auto-Updates");
            private MenuGroup _moreInfo_Commands = new MenuGroup("Commands");
            private MenuGroup _moreInfo_Menus = new MenuGroup($"Menus");

            private MenuItem _helpText_Commands = new MenuItem($"About commands:{Environment.NewLine}{Environment.NewLine}Each module can register multiple commands in GridOS.{Environment.NewLine}{Environment.NewLine}These commands can be executed by supplying their name to the programmable block as a parameter.{Environment.NewLine}{Environment.NewLine}Besides these, there are some built-in commands in GridOS, for example for registering LCDs, and enabling and disabling the automatic updates.{Environment.NewLine}{Environment.NewLine}You can find the list of all available commands in the Status menu.{Environment.NewLine}{Environment.NewLine}This command list dynamically updates, and always shows the actually available commands at a given moment.{Environment.NewLine}");

            public HelpMenu(string label = "System") : base(label)
            {
                ShowBackCommandAtBottom = false;

                Label = "Help";

                AddChild(_helpTextGeneric);
                AddChild(_moreInfo_Modules);
                AddChild(_moreInfo_AutoUpdates);
                AddChild(_moreInfo_Commands);
                AddChild(_moreInfo_Menus);

                _moreInfo_Commands.ShowBackCommandAtBottom = true;
                _moreInfo_Commands.AddChild(_helpText_Commands);

            }
        }
    }
}
