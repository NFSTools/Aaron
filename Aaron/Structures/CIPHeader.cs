using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    /// <summary>
    /// "Compression-In-Place" header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIPHeader
    {
        // =0x55441122
        public uint Magic;

        // =0x8000
        public int USize;

        // data size + sizeof(CIPHeader)
        public int CSize;

        public int UPos;

        public int CPos;

        public uint Null;
    }
}
