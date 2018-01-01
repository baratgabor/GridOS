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
        /// Generic notification wrapper for value types. Notifies subscribers when a new value has been assigned.
        /// </summary>
        /// <typeparam name="T">The type to be wrapped with notification logic</typeparam>
        public class Observable<T> where T : struct
        {
            public event Action<Observable<T>> PropertyChanged;
            private T _value;

            public Observable(T value)
            {
                _value = value;
            }

            public T Value
            {
                get { return _value; }
                set
                {
                    if (EqualityComparer<T>.Default.Equals(value, _value))
                        return;
                    _value = value;
                    PropertyChanged?.Invoke(this);
                }
            }
        }
    }
}