using System.Text;
using System;

namespace IngameScript
{
    partial class Program
    {
        class DisplayHeader : IControl
        {
            protected StringBuilder _buffer = new StringBuilder();
            protected ProgressIndicator2 _spinner = new ProgressIndicator2();
            IMyGridProgramRuntimeInfo _runtime;

            public event Action<IControl> RedrawRequired;

            public DisplayHeader(IBreadcrumbConfig config, IMyGridProgramRuntimeInfo runtime)
            {
                _runtime = runtime;
            }

            public StringBuilder GetContent(bool FlushCache = false)
            {
                _buffer.Clear();
                _buffer.AppendLine();
                _buffer.Append(" ::GridOS:: ");
                _buffer.Append(_spinner.Get());
                _buffer.Append(" LRT: ");
                _buffer.AppendFormat("{0:G3}ms", _runtime.LastRunTimeMs);
                return _buffer;
            }
        }
    }
}
