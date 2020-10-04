﻿using System.Collections.Generic;
using System.Text;
using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Sets up a TextSurface with the appropriate settings, and fills/refreshes it with the content of the Controls added.
        /// </summary>
        class DisplayView : IView
        {
            protected IMyTextSurface _target;
            protected IMyGridProgramRuntimeInfo _runtime;
            protected string _targetFont = "Debug";
            protected float _targetFontSize = 0.5f;
            protected int _maxLineWidth;
            protected int _maxLineNum = 15; // TODO: Check this max line number, and make it scale dynamically based on font size

            protected StringBuilder _buffer = new StringBuilder();
            protected List<IControl> _controls = new List<IControl>();
            protected List<string> _controls_cache = new List<string>();
            protected List<int> _controls_BufferStartPositions = new List<int>(); // Possible strategy for replacing only a single changed item, instead of rebuilding the buffer

            protected const char _lineSeparatorCharTop = '.';
            protected const char _lineSeparatorCharBottom = '˙';
            protected IViewConfig_Writeable _config;

            public DisplayView(IMyTextSurface target, IViewConfig_Writeable fillable_config, IMyGridProgramRuntimeInfo runtime)
            {
                _runtime = runtime;
                _target = target;
                _config = fillable_config;

                SetupTarget(_target);
                _config.LineLength = _maxLineWidth = DetermineMaxLineLength();
                _config.LineHeight = _maxLineNum;

                _config.PaddingChar = ' ';
                _config.PaddingLeft = 3;
                _config.SeparatorLineTop = new String(_lineSeparatorCharTop, _maxLineWidth * 2);
                _config.SeparatorLineBottom = new String(_lineSeparatorCharBottom, _maxLineWidth * 2);
            }

            public DisplayView AddControl(IControl control)
            {
                _controls.Add(control);
                control.RedrawRequired += Redraw;
                return this;
            }

            // TODO: Consider control disposal mechanisms? Or what's the assumption when we remove a control?
            public void RemoveControl(IControl control)
            {
                if (_controls.Contains(control))
                {
                    control.RedrawRequired -= Redraw;
                    _controls.Remove(control);
                }
            }

            public void ClearControls()
            {
                foreach (var c in _controls)
                    c.RedrawRequired -= Redraw;

                _controls.Clear();
            }

            private void SetupTarget(IMyTextSurface target)
            {
                target.Font = _targetFont;
                target.FontSize = _targetFontSize;
                target.ContentType = ContentType.TEXT_AND_IMAGE;
            }

            private int DetermineMaxLineLength()
            {
                // Proportional to X characters at 1.0 font size
                return (int)Math.Truncate(40 / _targetFontSize);
            }

            public void Redraw(StringBuilder content)
            {
                // TODO: Make the event payload meaningful; e.g. it needs to ID the source at least.
                // Currently we are simply ignoring the event payload (content), and requesting it again
                // But that's okay actually, since it's supposed to be cached

                // TODO: possibly implement caching, by using _controls_cache, if that improves the runtime due to eliminating one reference; but probably the diff. is negligible
                _buffer.Clear();
                //_controls_BufferStartPositions.Clear();
                for (int i = 0, pos = 0; i < _controls.Count; i++)
                {
                    //_controls_BufferStartPositions.Add(pos);
                    _buffer.Append(_controls[i].GetContent() + (i < _controls.Count-1 ? Environment.NewLine : ""));
                    //pos += _buffer.Length;
                }             

                // TODO: Should we do line number checks on the control outputs?
                // MaxLineNum is textsurface settings dependent, so this View will decide on that
                _target.WriteText(_buffer.ToString());
            }
        }
    }
}