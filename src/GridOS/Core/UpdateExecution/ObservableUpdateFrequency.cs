using System;

namespace IngameScript
{
    /// <summary>
    /// Wraps UpdateType to encapsulate notification logic.
    /// </summary>
    public class ObservableUpdateFrequency
    {
        public UpdateType UpdateTypeEquivalent { get; private set; }
        public delegate void FrequencyChangeHandler(IUpdateSubscriber sender, UpdateFrequency oldFrequency, UpdateFrequency newFrequency);
        public event FrequencyChangeHandler UpdateFrequencyChanged;

        private UpdateFrequency _updateFrequency;
        private readonly IUpdateSubscriber _updateSubscriber;

        public ObservableUpdateFrequency(UpdateFrequency initialUpdateFrequency, IUpdateSubscriber updateSubscriber)
        {
            _updateFrequency = initialUpdateFrequency;
            _updateSubscriber = updateSubscriber;
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
            UpdateFrequencyChanged?.Invoke(_updateSubscriber, oldUpdateFrequency, newUpdateFrequency);
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