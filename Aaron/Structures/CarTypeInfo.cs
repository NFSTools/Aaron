using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Aaron.Enums;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarTypeInfo
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string CarTypeName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string BaseModelName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string GeometryFilename;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ManufacturerName;
        public uint CarTypeNameHash;
        public float HeadlightFOV;
        public char padHighPerformance;
        public char NumAvailableSkinNumbers;
        public char WhatGame;
        public char ConvertibleFlag;
        public char WheelOuterRadius;
        public char WheelInnerRadiusMin;
        public char WheelInnerRadiusMax;
        public char pad0;
        public bVector3 HeadlightPosition;
        public bVector3 DriverRenderingOffset;
        public bVector3 InCarSteeringWheelRenderingOffset;
        public int Type; // this should really be called ID or Index or something... not Type! but I guess there's a reason...
        public CarUsageType UsageType;
        public uint CarMemTypeHash;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] MaxInstances;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] WantToKeepLoaded;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] pad4;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public float[] MinTimeBetweenUses;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] AvailableSkinNumbers;
        public byte DefaultSkinNumber;
        [MarshalAs(UnmanagedType.I1)]
        public bool Skinnable;
        public int Padding;
        public uint DefaultBasePaint;
    }
}
