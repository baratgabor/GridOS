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
        /// Extracts data needed for higher level components.
        /// </summary>
        class LineInfoExtractor : IProcessor<string, IEnumerable<LineInfo>>
        {
            protected List<LineInfo> _buffer = new List<LineInfo>();
            protected IPaddingConfig _config;

            public LineInfoExtractor(IPaddingConfig config)
            {
                _config = config;
            }

            public StringBuilder Process(string input, ProcessingConfiguration args, StringBuilder output, bool clearOutput = false)
            {
                // TODO: highly assumptious; bulletCharPosition is problematic, since we switched to prefix strings, which can be longer than 1 char.
                // also assumes that structure is "padding + prefix + element", but actually order can be any
                int bulletCharPosition = args.CurrentOutputLength + _config.PaddingLeft;

                for (int currentPos = 0; currentPos != -1; currentPos = input.IndexOf(Environment.NewLine, currentPos))
                {
                    if (currentPos != -1)
                    {
                        if (currentPos != 0) currentPos += Environment.NewLine.Length;
                        args.LineInfo.Add(
                            new LineInfo(
                                startPosition: currentPos + args.CurrentOutputLength,
                                parentMenuItem: args.Element,
                                bulletCharPosition: bulletCharPosition
                            )
                        );
                        currentPos++;
                    }
                }

                return output;
            }

            public IEnumerable<LineInfo> Process(string input)
            {
                _buffer.Clear();

                //int bulletCharPosition = args.CurrentOutputLength + _config.PaddingLeft;

                for (int currentPos = 0; currentPos != -1; currentPos = input.IndexOf(Environment.NewLine, currentPos))
                {
                    if (currentPos == -1)
                        break;

                    if (currentPos != 0)
                        currentPos += Environment.NewLine.Length;

                    _buffer.Add(
                        new LineInfo(
                  //          startPosition: currentPos + args.CurrentOutputLength,
                    //        parentMenuItem: args.Element,
                      //      bulletCharPosition: bulletCharPosition
                        )
                    );
                    currentPos++;
                }
                return _buffer;
            }
        }
    }
}
