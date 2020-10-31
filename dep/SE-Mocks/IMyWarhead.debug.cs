namespace IngameScript
{
    public interface IMyWarhead : IMyTerminalBlock, IMyCubeBlock, IMyEntity
    {
        bool IsCountingDown { get; }
        float DetonationTime { get; set; }
        bool IsArmed { get; set; }

        void Detonate();
        bool StartCountdown();
        bool StopCountdown();
    }
}
