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
        UpdateType ConvertUpdateFrequency(UpdateFrequency updateFrequency)
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

        UpdateFrequency ConvertUpdateType(UpdateType updateType)
        {
            UpdateFrequency updateFrequency = UpdateFrequency.None;

            if ((updateType & UpdateType.Once) != 0)
                updateFrequency |= UpdateFrequency.Once;

            if ((updateType & UpdateType.Update1) != 0)
                updateFrequency |= UpdateFrequency.Update1;

            if ((updateType & UpdateType.Update10) != 0)
                updateFrequency |= UpdateFrequency.Update10;

            if ((updateType & UpdateType.Update100) != 0)
                updateFrequency |= UpdateFrequency.Update100;

            return updateFrequency;
        }

    }
}
