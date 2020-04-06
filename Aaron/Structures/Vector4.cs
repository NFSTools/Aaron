using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Vector4
    {
        public float X, Y, Z, W;
    }
}
