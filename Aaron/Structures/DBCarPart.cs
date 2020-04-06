using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DBCarPart
    {
        public byte Unused;
        public short CarIndex;
        public byte Unused2;
        public int AttributeTableOffset;
        public uint Hash;
    }
}
