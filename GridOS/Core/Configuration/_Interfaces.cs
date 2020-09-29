using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
	{
        struct ContentChangeInfo
        {
            public readonly List<IMenuItem> Content;
            public readonly List<string> NavigationPath;
            public readonly IMenuGroup PreviousContext;

            public ContentChangeInfo(List<IMenuItem> content, List<string> navigationPath, IMenuGroup previousContext)
            {
                Content = content;
                NavigationPath = navigationPath;
                PreviousContext = previousContext;
            }
        }

        class ProcessingArgs
        {
            public char Prefix { get; set; }
            public char Suffix { get; set; }
            public List<LineInfo> LineInfo { get; set; }
            public IMenuItem Item { get; set; }
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
            char GetPrefixFor(IMenuItem item, bool selected);
            char GetSuffixFor(IMenuItem item, bool selected);
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
    }
}
