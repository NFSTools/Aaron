using System.Runtime.InteropServices;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarPartPack
    {
        public uint Version; //=6
        public uint pStringTable; //=0
        public uint StringTableSize; //=0
        public uint pAttributeTableTable; //=0
        public int NumAttributeTables;
        public uint pAttributesTable; //=0
        public int NumAttributes; //=14035
        public uint pTypeNameTable; //=0
        public int NumTypeNames;
        public uint pModelTable; //=0
        public int NumModelTables; //=0xA8
        public uint pPartsTable; //=0
        public int NumParts;
        public int StartIndex; //=0
    }
}
