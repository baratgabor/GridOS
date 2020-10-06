namespace IngameScript
{
    public struct MyDetectedEntityInfo
    {
        //
        // Summary:
        //     The entity's EntityId
        public readonly long EntityId;
        //
        // Summary:
        //     The entity's display name if it is friendly, or a generic descriptor if it is
        //     not
        public readonly string Name;
        //
        // Summary:
        //     Enum describing the type of entity
        //public readonly MyDetectedEntityType Type;
        //
        // Summary:
        //     Position where the raycast hit the entity. (can be null if the sensor didn't
        //     use a raycast)
        public readonly Vector3D? HitPosition;
        //
        // Summary:
        //     The entity's absolute orientation at the time it was detected
        //public readonly MatrixD Orientation;
        //
        // Summary:
        //     The entity's absolute velocity at the time it was detected
        public readonly Vector3 Velocity;
        //
        // Summary:
        //     Relationship between the entity and the owner of the sensor
        //public readonly MyRelationsBetweenPlayerAndBlock Relationship;
        //
        // Summary:
        //     The entity's world-aligned bounding box
        //public readonly BoundingBoxD BoundingBox;
        //
        // Summary:
        //     Time when the entity was detected. This field counts milliseconds, compensated
        //     for simspeed
        public readonly long TimeStamp;

        //
        // Summary:
        //     The entity's position (center of the Bounding Box)
        public Vector3D Position { get; }
    }
}
