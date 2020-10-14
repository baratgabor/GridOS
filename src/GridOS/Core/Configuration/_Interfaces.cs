namespace IngameScript
{
    interface IMenuPresentationConfig
    {
        int LineHeight { get; }
        int LineLength { get; }
        char[] WordDelimiters { get; }
        int PaddingLeft { get; }
        char PaddingChar { get; }
        char SelectionMarker { get; }
        
        AffixConfig Prefixes_Unselected { get; }
        AffixConfig Prefixes_Selected { get; }
        AffixConfig Suffixes_Unselected { get; }
        AffixConfig Suffixes_Selected { get; }
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
