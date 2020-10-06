namespace IngameScript
{
    public interface IMyCameraBlock : IMyFunctionalBlock, IMyTerminalBlock, IMyCubeBlock, IMyEntity
    {
        //
        // Summary:
        //     Determines whether this camera is currently in use.
        bool IsActive { get; }
        //
        // Summary:
        //     The maximum distance that this camera can scan, based on the time since the last
        //     scan.
        double AvailableScanRange { get; }
        //
        // Summary:
        //     When this is true, the available raycast distance will count up, and power usage
        //     is increased.
        bool EnableRaycast { get; set; }
        //
        // Summary:
        //     Returns the maximum positive angle you can apply for pitch and yaw.
        float RaycastConeLimit { get; }
        //
        // Summary:
        //     Returns the maximum distance you can request a raycast. -1 means infinite.
        double RaycastDistanceLimit { get; }

        //
        // Summary:
        //     Checks if the camera can scan the given distance.
        //
        // Parameters:
        //   distance:
        bool CanScan(double distance);
        //
        // Summary:
        //     Checks if the camera can scan to the given direction and distance.
        //
        // Parameters:
        //   distance:
        //
        //   direction:
        bool CanScan(double distance, Vector3D direction);
        //
        // Summary:
        //     Checks if the camera can scan to the given target
        //
        // Parameters:
        //   target:
        bool CanScan(Vector3D target);
        //
        // Summary:
        //     Does a raycast in the direction the camera is facing. Pitch and Yaw are in degrees.
        //     Will return an empty struct if distance or angle are out of bounds.
        //
        // Parameters:
        //   distance:
        //
        //   pitch:
        //
        //   yaw:
        MyDetectedEntityInfo Raycast(double distance, float pitch = 0, float yaw = 0);
        //
        // Summary:
        //     Does a raycast to the given point. Will return an empty struct if distance or
        //     angle are out of bounds.
        //
        // Parameters:
        //   targetPos:
        MyDetectedEntityInfo Raycast(Vector3D targetPos);
        //
        // Summary:
        //     Does a raycast in the given direction. Will return an empty struct if distance
        //     or angle are out of bounds.
        //
        // Parameters:
        //   distance:
        //
        //   targetDirection:
        MyDetectedEntityInfo Raycast(double distance, Vector3D targetDirection);
        //
        // Summary:
        //     Returns the number of milliseconds until the camera can do a raycast of the given
        //     distance.
        //
        // Parameters:
        //   distance:
        int TimeUntilScan(double distance);
    }
}
