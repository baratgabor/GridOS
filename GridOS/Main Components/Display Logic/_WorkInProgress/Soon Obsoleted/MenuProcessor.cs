using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
	partial class Program
	{
        /// <summary>
        /// Content builder that builds content as a single string from a list of elements
        /// </summary>
        class MenuProcessor : ILineInfoProviderControl
        {
            public List<LineInfo> LineInfo => _lineInfo;
            public event Action<StringBuilder> RedrawRequired;

            protected IMenuContentSource _contentSource;
            protected List<IDisplayElementProcessor> _pipeline = new List<IDisplayElementProcessor>();
            protected StringBuilder _menuBuffer = new StringBuilder();
            protected StringBuilder _itemBuffer = new StringBuilder();

            protected IAffixConfig _config;
            protected ProcessingConfiguration _processingArgs = new ProcessingConfiguration();
            protected List<LineInfo> _lineInfo = new List<LineInfo>();

            public MenuProcessor(IAffixConfig config, IMenuContentSource contentSource)
            {
                _config = config;
                _processingArgs.LineInfo = _lineInfo;

                _contentSource = contentSource;
                _contentSource.ContentChanged += OnContentChanged;
            }

            protected void Process()
            {
                _menuBuffer.Clear();

                // Honestly, we know that this IEnumerable is a List, so there won't be a lot of overhead with ToList()
                var menuItems = _contentSource.Content.ToList();

                if (menuItems == null || menuItems.Count == 0)
                    return;

                _lineInfo.Clear();

                for (int i = 0; i < menuItems.Count; i++)
                {
                    var item = menuItems[i];
                    _processingArgs.Prefix = _config.GetPrefixFor(item, false);
                    _processingArgs.Suffix = _config.GetSuffixFor(item, false);
                    _processingArgs.Element = item;
                    _processingArgs.CurrentOutputLength = _menuBuffer.Length;

                    _itemBuffer
                        .Clear() // Prepare item buffer for new item processing
                        .Append(item.Label); // Set initial value to be processed
                    
                    foreach (var p in _pipeline)
                    {
                        // Send initial value through the chain of processors
                        // Output is stored in the StringBuilder passed in
                        p.Process(_itemBuffer, item);
                    }

                    // Add processed item to the menu buffer
                    _menuBuffer.Append(_itemBuffer + Environment.NewLine);
                }
                // Remove the last NewLine (fastest method)
                _menuBuffer.Length -= 2;
            }

            protected void OnContentChanged(IEnumerable<IDisplayElement> content)
            {
                Process_Redraw();
            }

            protected void Process_Redraw()
            {
                Process();
                RedrawRequired?.Invoke(_menuBuffer);
            }

            public MenuProcessor AddProcessor(IDisplayElementProcessor processor)
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
                    // Update buffer with freshly pulled/processed content
                    Process();

                return _menuBuffer;
            }
        }
    }
}
