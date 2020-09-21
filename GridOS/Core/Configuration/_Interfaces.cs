using System.Collections.Generic;

namespace IngameScript
{
    partial class Program
	{
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
    }
}
