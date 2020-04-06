using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FEPresetPaint
    {
        public uint Group;
        public uint Hue;
        public byte Saturation;
        public byte Variance;
        public short Pad;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FEPresetVinylHue
    {
        public int Hue;
        public byte Saturation;
        public byte Variance;
        public short Blank;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FEPresetVinyl
    {
        public int Hash;
        public short TranX;
        public short TranY;
        public short ScaleX;
        public short ScaleY;
        public byte Rotation;
        public byte Shear;
        public short Blank;
        public uint Blank2;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public FEPresetVinylHue[] Hues;

        [MarshalAs(UnmanagedType.Bool)]
        public bool IsMirrored;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FEPresetCar
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] InheritedFields; // FEPresetCar : bTNode<FEPresetCar> probably

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string CarCollectionName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string PresetName;

        public uint VehicleKey;
        public uint InverseThing;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 123)]
        public uint[] VisualPartHashes;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 8)]
        public bool[] PaintsSet;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public FEPresetPaint[] Paints;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x64)]
        public byte[] Blank;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public uint[] PerformanceParts;

        public uint Blank2;

        public uint SkillModSlotCount;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public uint[] SkillModHashes;

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = 8)]
        public bool[] SkillModsFixed;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 31)]
        public FEPresetVinyl[] Vinyls;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Blank4;
    }
}
