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
        class ProcessingArgs
        {
            public string Prefix { get; set; }
            public string Suffix { get; set; }
            public List<LineInfo> LineInfo { get; set; }
            public IDisplayElement Element { get; set; }
            public int CurrentOutputLength { get; set; }
        }

        interface IWordWrappingConfig
        {
            int LineLength { get; }
            char[] Terminators { get; }
        }

        interface IViewportConfig
        {
            int LineHeight { get; }
        }

        interface IPaddingConfig
        {
            char PaddingChar { get; }
            int PaddingLeft { get; }
        }

        interface IBreadcrumbConfig : IPaddingConfig
        {
            string PathSeparator { get; }
        }

        interface INavConfig
        {
            char SelectionMarker { get; }
        }

        interface IAffixConfig
        {
            string GetPrefixFor(IDisplayElement element, bool selected);
            string GetSuffixFor(IDisplayElement element, bool selected);
        }

        class SmartConfig : IWordWrappingConfig, IViewportConfig, IPaddingConfig, INavConfig, IAffixConfig, IBreadcrumbConfig
        {
            public int LineLength { get; set; }
            public char[] Terminators { get; set; }

            public int LineHeight { get; set; }

            public char PaddingChar { get; set; }
            public int PaddingLeft { get; set; }

            public string PathSeparator { get; set; }

            public char SelectionMarker { get; set; } = '›';

            public Affix Prefixes_Unselected = new Affix()
            {
                Element = " ",
                Command = "·",
                Group = "·"
            };

            public Affix Suffixes_Unselected = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };
            public Affix Prefixes_Selected = new Affix()
            {
                Element = " ",
                Command = "•",
                Group = "•"
            };
            public Affix Suffixes_Selected = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };

            public string GetPrefixFor(IDisplayElement element, bool selected)
            {
                if (selected) return GetAffix(element, selected, Prefixes_Selected);
                else return GetAffix(element, selected, Prefixes_Unselected);
            }

            public string GetSuffixFor(IDisplayElement element, bool selected)
            {
                if (selected) return GetAffix(element, selected, Suffixes_Selected);
                else return GetAffix(element, selected, Suffixes_Unselected);
            }

            protected string GetAffix(IDisplayElement element, bool selected, Affix affix)
            {
                string value = "";

                if (element is IDisplayGroup)
                    value = affix.Group;
                else if (element is IDisplayCommand)
                    value = affix.Command;
                else if (element is IDisplayElement)
                    value = affix.Element;

                return value;
            }

            public struct Affix
            {
                public string Group;
                public string Command;
                public string Element;
            }
        }
    }
}
