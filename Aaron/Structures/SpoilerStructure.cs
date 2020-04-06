using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpoilerStructure
    {
        public uint CarTypeNameHash;

        public uint Unknown; // 3C 00 00 00

        public uint CarTypeNameHash2; //=CarTypeNameHash

        /// <summary>
        /// bin hash of spoiler model name
        /// </summary>
        /// <example>
        /// E8E9C8A4 = SPOILER_HATCH
        /// E93222A7 = SPOILER_LARGE
        /// E9B71B15 = SPOILER_SMALL
        /// 5C67B1EC = SPOILER_NONE
        /// </example>
        public uint SpoilerHash;

        // 0x14 null bytes (20 bytes)
        public uint Null;
        public uint Null2;
        public uint Null3;
        public uint Null4;
        public uint Null5;
    }
}
