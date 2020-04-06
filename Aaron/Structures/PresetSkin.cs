using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PresetSkin
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x8)]
        public byte[] Null;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string PresetName;

        public uint PaintGroup;
        public uint PaintHue;
        public byte PaintSaturation;
        public byte PaintVariance;
        public ushort Null2;

        public uint VinylHash; // set to FFFFFFFF

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x38)]
        public byte[] Null3;
    }
}
