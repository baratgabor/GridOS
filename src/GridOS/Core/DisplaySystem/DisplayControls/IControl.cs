using System.Text;
using System;

namespace IngameScript
{
    partial class Program
	{
        interface IControl
        {
            event Action<IControl> RedrawRequired;
            StringBuilder GetContent(bool FlushCache = false);
        }
    }
}
