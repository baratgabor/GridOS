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
        // TODO: Create proper structure for everything in this file

        struct ContentChangeInfo
        {
            public readonly List<IDisplayElement> Content;
            public readonly List<string> NavigationPath;
            public readonly IDisplayGroup PreviousContext;

            public ContentChangeInfo(List<IDisplayElement> content, List<string> navigationPath, IDisplayGroup previousContext)
            {
                Content = content;
                NavigationPath = navigationPath;
                PreviousContext = previousContext;
            }
        }

        class ProcessingConfiguration
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
            int PaddingLeft_FirstLine { get; }
        }

        interface IBreadcrumbConfig : IPaddingConfig
        {
            string PathSeparator { get; }
            string SeparatorLineTop { get; }
            string SeparatorLineBottom { get; }
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

        interface IViewConfig_Writeable
        {
            int LineLength { get; set; }
            int LineHeight { get; set; }
            char PaddingChar { get; set; }
            int PaddingLeft { get; set; }
            int PaddingLeft_FirstLine { get; set; }
            string PathSeparator { get; set; }
            char SelectionMarker { get; set; }
            string SeparatorLineTop { get; set; }
            string SeparatorLineBottom { get; set; }
        }

        class SmartConfig : IWordWrappingConfig, IViewportConfig, IPaddingConfig, INavConfig, IAffixConfig, IBreadcrumbConfig, IViewConfig_Writeable
        {
            public int LineLength { get; set; }
            public char[] Terminators { get; set; } = {' ', '-'};

            public int LineHeight { get; set; }

            public char PaddingChar { get; set; } = ' ';
            public int PaddingLeft { get; set; } = 0;
            public int PaddingLeft_FirstLine { get; set; }

            public string PathSeparator { get; set; } = "›";
            public string SeparatorLineTop { get; set; }
            public string SeparatorLineBottom { get; set; }

            public char SelectionMarker { get; set; } = '›';

            public Affix Prefixes_Unselected { get; set; } = new Affix()
            {
                Element = " ",
                Command = "·",
                Group = "·"
            };

            public Affix Suffixes_Unselected { get; set; } = new Affix()
            {
                Element = "",
                Command = "",
                Group = "»"
            };
            public Affix Prefixes_Selected { get; set; } = new Affix()
            {
                Element = " ",
                Command = "•",
                Group = "•"
            };
            public Affix Suffixes_Selected { get; set; } = new Affix()
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
                else
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
