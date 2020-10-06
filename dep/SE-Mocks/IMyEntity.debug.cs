namespace IngameScript
{
    //
    // Summary:
    //     Ingame (Programmable Block) interface for all entities.
    public interface IMyEntity
    {
        //MyEntityComponentContainer Components { get; }
        long EntityId { get; }
        string Name { get; }
        string DisplayName { get; }
        //
        // Summary:
        //     Returns true if this entity has got at least one inventory. Note that one aggregate
        //     inventory can contain zero simple inventories => zero will be returned even if
        //     GetInventory() != null.
        bool HasInventory { get; }
        //
        // Summary:
        //     Returns the count of the number of inventories this entity has.
        int InventoryCount { get; }
        //BoundingBoxD WorldAABB { get; }
        //BoundingBoxD WorldAABBHr { get; }
        //MatrixD WorldMatrix { get; }
        //BoundingSphereD WorldVolume { get; }
        //BoundingSphereD WorldVolumeHr { get; }

        //
        // Summary:
        //     Simply get the MyInventoryBase component stored in this entity.
        //IMyInventory GetInventory();
        //
        // Summary:
        //     Search for inventory component with maching index.
        //IMyInventory GetInventory(int index);
        Vector3D GetPosition();
    }
}
