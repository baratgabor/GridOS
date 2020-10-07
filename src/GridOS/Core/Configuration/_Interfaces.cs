namespace IngameScript
{
    partial class Program
	{
        interface IMenuPresentationConfig
        {
            int LineHeight { get; }
            int LineLength { get; }
            char[] WordDelimiters { get; }
            int PaddingLeft { get; }
            char PaddingChar { get; }
            char SelectionMarker { get; }
            char GetPrefixFor(IMenuItem item, bool selected);
            char GetSuffixFor(IMenuItem item, bool selected);
        }

        interface IBreadcrumbConfig
        {
            string PathSeparator { get; }
            string SeparatorLineTop { get; }
            string SeparatorLineBottom { get; }
            int PaddingLeft { get; }
            char PaddingChar { get; }
        }

        interface IViewConfig_Writeable
        {
            int LineLength { get; set; }
            int LineHeight { get; set; }
            char PaddingChar { get; set; }
            int PaddingLeft { get; set; }
            string PathSeparator { get; set; }
            char SelectionMarker { get; set; }
            string SeparatorLineTop { get; set; }
            string SeparatorLineBottom { get; set; }
        }
    }
}
