using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Bounds
    {
        public Q4c Orientation;
        public V3c Position;
        public ushort Flags;
        public V3c HalfDimensions;
        public byte NumChildren;
        public byte PCloudIndex;
        public V3c Pivot;
        public short ChildIndex;
        public uint AttributeName;
        public uint Surface;
        public uint NameHash;
        public uint CollectionPtr; //=0
    }
}
