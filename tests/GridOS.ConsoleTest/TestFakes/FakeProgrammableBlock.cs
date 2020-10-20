using System;
using System.Text;

namespace IngameScript
{
    class FakeProgrammableBlock : IMyProgrammableBlock
    {
        public bool IsRunning => true;

        public string TerminalRunArgument => string.Empty;

        public bool Enabled { get; set; } = true;
        public bool ShowInInventory { get; set; } = true;
        public bool ShowInTerminal { get; set; } = true;
        public bool ShowOnHUD { get; set; } = true;
        public string CustomData { get; set; } = string.Empty;

        public string CustomInfo => string.Empty;

        public bool ShowInToolbarConfig { get; set; } = true;
        public string CustomName { get; set; } = "Programmable Block";

        public string CustomNameWithFaction => "Programmable Block";

        public string DetailedInfo => string.Empty;

        public string DisplayNameText => string.Empty;

        public int NumberInGrid => 1;

        public float Mass => 10;

        public bool IsWorking => true;

        public bool IsFunctional => true;

        public bool IsBeingHacked => false;

        public long OwnerId => 1;

        public string DefinitionDisplayNameText => string.Empty;

        public IMyCubeGrid CubeGrid => throw new NotImplementedException();

        public bool CheckConnectionAllowed => true;

        public float DisassembleRatio => 1;

        public long EntityId => 1;

        public string Name => "Programmable Block";

        public string DisplayName => "Programmable Block";

        public bool HasInventory => false;

        public int InventoryCount => 0;

        public int SurfaceCount => 2;

        public string GetOwnerFactionTag()
        {
            throw new NotImplementedException();
        }

        public Vector3D GetPosition()
        {
            throw new NotImplementedException();
        }

        public IMyTextSurface GetSurface(int index)
        {
            throw new NotImplementedException();
        }

        public bool HasLocalPlayerAccess()
        {
            throw new NotImplementedException();
        }

        public bool HasPlayerAccess(long playerId)
        {
            throw new NotImplementedException();
        }

        public bool IsSameConstructAs(IMyTerminalBlock other)
        {
            throw new NotImplementedException();
        }

        public void RequestEnable(bool enable)
        {
            throw new NotImplementedException();
        }

        public void SetCustomName(StringBuilder text)
        {
            throw new NotImplementedException();
        }

        public void SetCustomName(string text)
        {
            throw new NotImplementedException();
        }

        public bool TryRun(string argument)
        {
            throw new NotImplementedException();
        }

        public void UpdateIsWorking()
        {
            throw new NotImplementedException();
        }

        public void UpdateVisual()
        {
            throw new NotImplementedException();
        }
    }
}
