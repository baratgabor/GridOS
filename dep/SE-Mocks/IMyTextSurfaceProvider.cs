namespace IngameScript
{
    public interface IMyTextSurfaceProvider
    {
        int SurfaceCount { get; }

        IMyTextSurface GetSurface(int index);
    }
}
