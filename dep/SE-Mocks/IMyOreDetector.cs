namespace IngameScript
{
    public interface IMyOreDetector : IMyFunctionalBlock, IMyTerminalBlock, IMyCubeBlock, IMyEntity
    {
        float Range { get; }
        bool BroadcastUsingAntennas { get; set; }
    }
}
