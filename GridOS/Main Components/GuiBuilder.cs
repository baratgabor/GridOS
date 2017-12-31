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
        /* * 
         * GUI Builder class, planned roles:
         * - intermediate component between display components and the actualy TextPanel -- should this class write to the textpanel directly?
         * - facilitates the building of TextPanel content, freeing up display components from string operations
         * - among other things, implements TextPanel content scrolling; planned functionality:
         *      - if there are selectable items, it should keep currently selected item in "viewport"
         *      - if there are no selectable items, possibly it should still keep track of an "invisible" selected item to facilitate content scrolling
         *      - consequently, this class should receive which item is currently selected, from e.g. the CommandMenu instance
         *      - possibly need to rethink the structure... maybe navigation command handling shouldn't be the responsibility of the CommandMenu?
         *  
         *  - Consider developing some sort of compatibility with well-known scripts, e.g. MMaster's inventory script...
         *  - Need to check how modular these scripts, whether their output can be plugged in in some way 
         * 
         * */

        class GuiBuilder
        {
            public GuiBuilder()
            {
                throw new Exception("Not implemented");
            }

            void AddLine()
            {

            }

            void AddSelectableLine()
            {

            }

            void AddSeparatorLine()
            {

            }

            void MoveSelectionDown()
            { }

            void MoveSelectionUP()
            { }

            // What about moving selection left and right? Implementing alignment?
            // Maybe selectable items shouldn't be simple strings, but instead keyvaluepair, so the key of the item could be passed instead?

        }
    }
}
