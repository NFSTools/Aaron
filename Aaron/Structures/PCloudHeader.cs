using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PCloudHeader
    {
        public int NumPClouds;
        public int Pad, Pad2, Pad3;
    }
}
