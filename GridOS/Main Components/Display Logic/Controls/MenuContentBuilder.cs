﻿using Sandbox.Game.EntityComponents;
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
        class MenuContentBuilder : ILineInfoProviderControl
        {
            public List<LineInfo> LineInfo => _lineInfo;
            public event Action<StringBuilder> RedrawRequired;
            public Func<List<IDisplayElement>> ContentSource;

            protected List<IDisplayElement> _menuItems;
            protected List<ITextProcessor> _pipeline = new List<ITextProcessor>();
            protected StringBuilder _buffer = new StringBuilder();

            protected IAffixConfig _config;
            protected ProcessingArgs _processingArgs = new ProcessingArgs();
            protected List<LineInfo> _lineInfo = new List<LineInfo>();

            public MenuContentBuilder(IAffixConfig config)
            {
                _config = config;
                _processingArgs.LineInfo = _lineInfo;
            }

            public StringBuilder Process()
            {
                _buffer.Clear();
                _lineInfo.Clear();

                foreach (var e in _menuItems)
                {
                    _processingArgs.Prefix = _config.GetPrefixFor(e, false);
                    _processingArgs.Suffix = _config.GetSuffixFor(e, false);
                    _processingArgs.Element = e;
                    _processingArgs.CurrentOutputLength = _buffer.Length;

                    string s = e.Label;
                    foreach (var p in _pipeline)
                    {
                        // TODO: Eliminate ToString(); use only StringBuilders
                        s = p.Process(s, _processingArgs).ToString();
                    }
                    _buffer.Append(s + Environment.NewLine);
                }
                _buffer.Remove(_buffer.Length - 2, 2); // Remove last '\r\n' newline
                return _buffer;
            }

            public MenuContentBuilder AddContent(List<IDisplayElement> content)
            {
                _menuItems = content;
                Process_Redraw();
                return this;
            }

            protected void Add_Process_NoDraw(List<IDisplayElement> content)
            {
                _menuItems = content;
                Process();
            }

            internal void OnElementChanged(IDisplayElement obj)
            {
                Process_Redraw();
            }

            protected void Process_Redraw()
            {
                Process();
                RedrawRequired?.Invoke(_buffer);
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

                return _buffer;
            }

            public void OnContentChanged(List<IDisplayElement> elements)
            {
                AddContent(elements);
            }
        }
    }
}
