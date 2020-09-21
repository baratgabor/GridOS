using System.Text;

namespace IngameScript
{
    partial class Program
    {
        interface IView
        {
            DisplayView AddControl(IControl control);
            void RemoveControl(IControl control);
            void ClearControls();
            void Redraw(StringBuilder content);
        }
    }
}
