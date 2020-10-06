using System.Text;

namespace IngameScript
{
    public interface IMyTextPanel : IMyTextSurface, IMyFunctionalBlock, IMyTerminalBlock, IMyCubeBlock, IMyEntity
    {
        //
        // Summary:
        //     Returns true if the ShowOnScreen flag is set to either PUBLIC or PRIVATE
        bool ShowText { get; }

        string GetPrivateText();
        string GetPrivateTitle();
        string GetPublicText();
        string GetPublicTitle();
        void ReadPublicText(StringBuilder buffer, bool append = false);
        void ShowPrivateTextOnScreen();
        void ShowPublicTextOnScreen();
        void ShowTextureOnScreen();
        bool WritePrivateText(string value, bool append = false);
        bool WritePrivateTitle(string value, bool append = false);
        bool WritePublicText(string value, bool append = false);
        bool WritePublicText(StringBuilder value, bool append = false);
        bool WritePublicTitle(string value, bool append = false);
    }
}
