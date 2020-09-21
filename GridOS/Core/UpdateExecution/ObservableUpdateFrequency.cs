using Sandbox.ModAPI.Ingame;
using System;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Wraps UpdateType to encapsulate notification logic.
        /// </summary>
        public class ObservableUpdateFrequency
        {
            private UpdateFrequency _updateFrequency;
            public UpdateType UpdateTypeEquivalent { get; private set; }
            public event Action<ObservableUpdateFrequency, UpdateFrequency, UpdateFrequency> UpdateFrequencyChanged;

            public ObservableUpdateFrequency(UpdateFrequency initialUpdateFrequency)
            {
                _updateFrequency = initialUpdateFrequency;
                UpdateTypeEquivalent = ConvertUpdateFrequency(_updateFrequency);
            }

            public UpdateFrequency Get()
            {
                return _updateFrequency;
            }

            public void Set(UpdateFrequency newUpdateFrequency)
            {
                if (newUpdateFrequency == _updateFrequency)
                    return;

                UpdateFrequency oldUpdateFrequency = _updateFrequency;
                _updateFrequency = newUpdateFrequency;
                UpdateTypeEquivalent = ConvertUpdateFrequency(_updateFrequency);
                UpdateFrequencyChanged?.Invoke(this, oldUpdateFrequency, newUpdateFrequency);
            }

            private UpdateType ConvertUpdateFrequency(UpdateFrequency updateFrequency)
            {
                UpdateType updateType = UpdateType.None;

                if ((updateFrequency & UpdateFrequency.Once) != 0)
                    updateType |= UpdateType.Once;
                if ((updateFrequency & UpdateFrequency.Update1) != 0)
                    updateType |= UpdateType.Update1;
                if ((updateFrequency & UpdateFrequency.Update10) != 0)
                    updateType |= UpdateType.Update10;
                if ((updateFrequency & UpdateFrequency.Update100) != 0)
                    updateType |= UpdateType.Update100;

                return updateType;
            }
        }
    }
}