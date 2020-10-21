using EmptyKeys.UserInterface.Generated.DataTemplatesStoreBlock_Bindings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using VRage.Game.GUI.TextPanel;
using VRage.Utils;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private readonly GridOS gridOS;

        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.

            gridOS = new GridOS(this);
            gridOS.RegisterModule(new TestModule());
            gridOS.RegisterTextSurface(Me.GetSurface(0));
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.

            gridOS.Main(argument, updateSource);
        }

        public class TestModule : IModule, IMenuContentPublisher, IUpdateSubscriber
        {
            public IEnumerable<IMenuItem> MenuItems => menuItems;
            public string ModuleDisplayName => "TestModule1";

            public ObservableUpdateFrequency Frequency => new ObservableUpdateFrequency(UpdateFrequency.Update100, this);

            private readonly MenuItem _updatingMenuItem = new MenuItem("Updating");
            private readonly Random _rnd = new Random();

            private readonly List<IMenuItem> menuItems = new List<IMenuItem>()
            {
                new MenuItem("Test1"),
                new MenuItem("Test2"),
                new MenuItem("Test3"),
                new MenuItem("Test4"),
                new MenuItem("Test5"),
                new MenuItem("This is a longer text to test word wrapping in the menu.")
            };

            public TestModule()
            {
                menuItems.Add(_updatingMenuItem);
            }

            public void Update(UpdateType updateType)
            {
                // Showcase/test menu item updating.
                _updatingMenuItem.Label = $"Updating {_rnd.Next(0, 100)}";
            }
        }
    }
}
