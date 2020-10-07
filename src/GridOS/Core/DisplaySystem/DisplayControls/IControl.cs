using System.Text;
using System;

namespace IngameScript
{
    partial class Program
	{
        interface IControl
        {
            event Action<StringBuilder> RedrawRequired;
            StringBuilder GetContent(bool FlushCache = false);
        }
    }
}
