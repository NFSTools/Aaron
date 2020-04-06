using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Q4c
    {
        public short X, Y, Z, W;
    }
}
