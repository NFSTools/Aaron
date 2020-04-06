using Aaron.Structures;

namespace Aaron.Data.Bounds
{
    /// <summary>
    /// Represents an entry in a bounds pack.
    /// </summary>
    public class AaronBoundsEntry
    {
        public Q4c Orientation { get; set; }
        public V3c Position { get; set; }
        public AaronBoundsFlags Flags { get; set; }
        public V3c HalfDimensions { get; set; }
        public byte NumChildren { get; set; }
        public byte PCloudIndex { get; set; }
        public V3c Pivot { get; set; }
        public short ChildIndex { get; set; }
        public uint AttributeName { get; set; }
        public uint Surface { get; set; }
        public uint NameHash { get; set; }
    }
}
