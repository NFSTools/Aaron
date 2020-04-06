using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BoundsHeader
    {
        public uint NameHash;
        public int NumBounds;
        public int IsResolved; //=0
        public int Pad; //=0
    }
}
