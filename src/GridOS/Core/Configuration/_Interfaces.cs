using System;

namespace IngameScript
{
    public interface IDisplayConfig
    {
        float BaseFontSize { get; }
        string BaseFontName { get;}

        Color BaseFontColor { get; }
        Color BaseBackgroundColor { get; }

        float BaseLineHeight { get; }
        float BaseLineSpacing { get; }

        IMyTextSurface OutputSurface { get; }
        float OutputWidth { get; }
        float OutputHeight { get; }

        event Action<string> SettingChanged;
    }

    public interface IMenuPresentationConfig
    {
        int PaddingLeft { get; }
        char PaddingChar { get; }
        char SelectionMarker { get; }
        
        AffixConfig Prefixes_Unselected { get; }
        AffixConfig Prefixes_Selected { get; }
        AffixConfig Suffixes { get; }

        event Action<string> SettingChanged;
    }

    public interface IBreadcrumbConfig
    {
        string PathSeparator { get; }
        int PaddingLeft { get; }
        char PaddingChar { get; }
    }
}
