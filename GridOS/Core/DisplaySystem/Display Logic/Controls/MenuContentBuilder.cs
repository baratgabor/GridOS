using System.Collections.Generic;
using System.Text;
using System;

namespace IngameScript
{
    partial class Program
	{
        /// <summary>
        /// Content builder that builds content as a single string from a list of items
        /// </summary>
        class MenuContentBuilder : ILineInfoProviderControl
        {
            public List<LineInfo> LineInfo => _lineInfo;
            public event Action<StringBuilder> RedrawRequired;
            public Func<List<IMenuItem>> ContentSource;

            protected List<IMenuItem> _menuItems;
            protected List<ITextProcessor> _pipeline = new List<ITextProcessor>();
            protected StringBuilder _menuBuffer = new StringBuilder();
            protected StringBuilder _itemBuffer = new StringBuilder();

            protected IMenuPresentationConfig _config;
            protected ProcessingArgs _processingArgs = new ProcessingArgs();
            protected List<LineInfo> _lineInfo = new List<LineInfo>();

            public MenuContentBuilder(IMenuPresentationConfig config)
            {
                _config = config;
                _processingArgs.LineInfo = _lineInfo;
            }

            public StringBuilder Process()
            {
                _menuBuffer.Clear();
                _lineInfo.Clear();

                if (_menuItems == null)
                    return _menuBuffer;

                for (int i = 0; i < _menuItems.Count; i++)
                {
                    var item = _menuItems[i];
                    _processingArgs.Prefix = _config.GetPrefixFor(item, false);
                    _processingArgs.Suffix = _config.GetSuffixFor(item, false);
                    _processingArgs.Item = item;
                    _processingArgs.CurrentOutputLength = _menuBuffer.Length;

                    _itemBuffer
                        .Clear() // Prepare item buffer for new item processing
                        .Append(item.Label); // Set initial item value
                    
                    foreach (var p in _pipeline)
                    {
                        // Send initial value through the chain of processors
                        // Output is stored in the StringBuilder passed in
                        p.Process(_itemBuffer, _processingArgs);
                    }

                    // Add processed item to the menu buffer
                    // Conditionally add a newline if it's not the last item
                    _menuBuffer.Append(_itemBuffer + (i+1 >= _menuItems.Count ? "" : Environment.NewLine));
                }

                return _menuBuffer;
            }

            public MenuContentBuilder AddContent(List<IMenuItem> content)
            {
                _menuItems = content;
                Process_Redraw();
                return this;
            }

            protected void Add_Process_NoDraw(List<IMenuItem> content)
            {
                _menuItems = content;
                Process();
            }

            internal void OnItemChanged(IMenuItem _)
            {
                Process_Redraw();
            }

            protected void Process_Redraw()
            {
                Process();
                RedrawRequired?.Invoke(_menuBuffer);
            }

            public MenuContentBuilder AddProcessor(ITextProcessor processor)
            {
                _pipeline.Add(processor);
                return this;
            }

            public void ClearProcessors()
            {
                _pipeline.Clear();
            }

            public StringBuilder GetContent(bool FlushCache = false)
            {
                if (FlushCache)
                {
                    if (ContentSource == null)
                        throw new Exception("Content pull request failed: no content source set.");

                    // Buffer updated with freshly pulled/processed content
                    Add_Process_NoDraw(ContentSource.Invoke());
                }

                return _menuBuffer;
            }

            public void OnContentChanged(List<IMenuItem> items)
            {
                AddContent(items);
            }
        }
    }
}
