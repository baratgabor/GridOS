namespace IngameScript
{
    //
    // Summary:
    //     Grid interface
    public interface IMyCubeGrid : IMyEntity
    {
        //
        // Summary:
        //     Display name of the grid (as seen in Info terminal tab)
        string CustomName { get; set; }
        //
        // Summary:
        //     Grid size in meters
        float GridSize { get; }
        //
        // Summary:
        //     Grid size enum
        //MyCubeSize GridSizeEnum { get; }
        //
        // Summary:
        //     Determines if the grid is static (unmoveable)
        bool IsStatic { get; }
        //
        // Summary:
        //     Maximum coordinates of blocks in grid
        //Vector3I Max { get; }
        //
        // Summary:
        //     Minimum coordinates of blocks in grid
        //Vector3I Min { get; }

        //
        // Summary:
        //     Returns true if there is any block occupying given position
        //bool CubeExists(Vector3I pos);
        //
        // Summary:
        //     Get cube block at given position
        //
        // Parameters:
        //   pos:
        //     Block position
        //
        // Returns:
        //     Block or null if none is present at given position
        //IMySlimBlock GetCubeBlock(Vector3I pos);
        //
        // Summary:
        //     Converts grid coordinates to world space
        //Vector3D GridIntegerToWorld(Vector3I gridCoords);
        //
        // Summary:
        //     Determines whether this grid is mechanically connected to the other. This is
        //     any grid connected with rotors or pistons or other mechanical devices, but not
        //     things like connectors. This will in most cases constitute your complete construct.
        //     Be aware that using merge blocks combines grids into one, so this function will
        //     not filter out grids connected that way. Also be aware that detaching the heads
        //     of pistons and rotors will cause this connection to change.
        //
        // Parameters:
        //   other:
        bool IsSameConstructAs(IMyCubeGrid other);
        //
        // Summary:
        //     Converts world coordinates to grid space cell coordinates
        //Vector3I WorldToGridInteger(Vector3D coords);
    }
}
