using System.Text;
using System;

namespace IngameScript
{
    interface IControl
    {
        event Action<IControl> RedrawRequired;
        StringBuilder GetContent(bool FlushCache = false);
    }
}
