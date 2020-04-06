using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CIPFileHeader
    {
        public uint Magic;
        public int USize;
        public int CSize;
        public uint Unknown;
    }
}
