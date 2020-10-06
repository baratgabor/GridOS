namespace IngameScript
{
    public interface IMyMotorStator : /*IMyMotorBase, IMyMechanicalConnectionBlock,*/ IMyFunctionalBlock, IMyTerminalBlock, IMyCubeBlock, IMyEntity
    {
        //
        // Summary:
        //     Gets the current angle of the rotor in radians.
        float Angle { get; }
        //
        // Summary:
        //     Gets or sets the torque applied when moving the rotor top
        float Torque { get; set; }
        //
        // Summary:
        //     Gets or sets the torque applied when stopping the rotor top
        float BrakingTorque { get; set; }
        //
        // Summary:
        //     Gets or sets the desired velocity of the rotor in radians/second
        float TargetVelocityRad { get; set; }
        //
        // Summary:
        //     Gets or sets the desired velocity of the rotor in RPM
        float TargetVelocityRPM { get; set; }
        //
        // Summary:
        //     Gets or sets the lower angle limit of the rotor in radians. Set to float.MinValue
        //     for no limit.
        float LowerLimitRad { get; set; }
        //
        // Summary:
        //     Gets or sets the lower angle limit of the rotor in degrees. Set to float.MinValue
        //     for no limit.
        float LowerLimitDeg { get; set; }
        //
        // Summary:
        //     Gets or sets the upper angle limit of the rotor in radians. Set to float.MaxValue
        //     for no limit.
        float UpperLimitRad { get; set; }
        //
        // Summary:
        //     Gets or sets the upper angle limit of the rotor in degrees. Set to float.MaxValue
        //     for no limit.
        float UpperLimitDeg { get; set; }
        //
        // Summary:
        //     Gets or sets the vertical displacement of the rotor top
        float Displacement { get; set; }
        //
        // Summary:
        //     Gets or sets rotor lock
        bool RotorLock { get; set; }
    }
}
