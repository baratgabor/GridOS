using System.Text;
using System;

namespace IngameScript
{
    class DisplayHeader : IControl
    {
        protected readonly StringBuilder _buffer = new StringBuilder();
        protected readonly ProgressIndicator2 _spinner = new ProgressIndicator2();
        protected readonly IDiagnosticService _diagnostics;

        public event Action<IControl> RedrawRequired;

        public DisplayHeader(IBreadcrumbConfig config, IDiagnosticService diagnostics)
        {
            _diagnostics = diagnostics;
        }

        public StringBuilder GetContent(bool FlushCache = false)
        {
            _buffer.Clear();
            _buffer.Append(" ::GridOS:: ");
            _buffer.Append(_spinner.Get());
            _buffer.Append(" LRT: ");
            _buffer.AppendFormat("{0:G3}ms", _diagnostics.LastRunTimeMs);
            return _buffer;
        }
    }
}
