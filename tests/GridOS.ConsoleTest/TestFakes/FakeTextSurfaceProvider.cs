using IngameScript;
using System.Collections.Generic;

namespace GridOS.ConsoleTest.TestFakes
{
    class FakeTextSurfaceProvider : FakeTerminalBlock, IMyTextSurfaceProvider
    {
        public List<IMyTextSurface> Surfaces { get; set; } = new List<IMyTextSurface>()
        {
            new FakeDisplay() { DisplayName = "Block Lcd 1" },
            new FakeDisplay() { DisplayName = "Block Lcd 2" },
            new FakeDisplay() { DisplayName = "Block Lcd 3" },
        };

        public int SurfaceCount => Surfaces.Count;

        public FakeTextSurfaceProvider()
        {
            DisplayName = "BlockName";
        }

        public IMyTextSurface GetSurface(int index)
        {
            return Surfaces[index];
        }
    }
}
