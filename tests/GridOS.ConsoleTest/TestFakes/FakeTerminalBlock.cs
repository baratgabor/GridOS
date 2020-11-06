using System;
using System.Text;

namespace IngameScript
{
    class FakeTerminalBlock : IMyTerminalBlock
    {
        public bool ShowInInventory { get; set; }
        public bool ShowInTerminal { get; set; }
        public bool ShowOnHUD { get; set; }
        public string CustomData { get; set; }

        public string CustomInfo { get; set; }

        public bool ShowInToolbarConfig { get; set; }
        public string CustomName { get; set; }

        public string CustomNameWithFaction { get; set; }

        public string DetailedInfo { get; set; }

        public string DisplayNameText { get; set; }

        public int NumberInGrid { get; set; }

        public float Mass { get; set; }

        public bool IsWorking { get; set; }

        public bool IsFunctional { get; set; }

        public bool IsBeingHacked { get; set; }

        public long OwnerId { get; set; }

        public string DefinitionDisplayNameText { get; set; }

        public IMyCubeGrid CubeGrid { get; set; }

        public bool CheckConnectionAllowed { get; set; }

        public float DisassembleRatio { get; set; }

        public long EntityId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool HasInventory { get; set; }

        public int InventoryCount { get; set; }

        public string GetOwnerFactionTag()
        {
            throw new NotImplementedException();
        }

        public Vector3D GetPosition()
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

        public void SetCustomName(StringBuilder text)
        {
            throw new NotImplementedException();
        }

        public void SetCustomName(string text)
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
