using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct CarPartAttribute : IEquatable<CarPartAttribute>
    {
        [FieldOffset(0)]
        public uint NameHash;

        [FieldOffset(4)]
        public float fParam;

        [FieldOffset(4)]
        public int iParam;

        [FieldOffset(4)]
        public uint uParam;

        public bool Equals(CarPartAttribute other)
        {
            return NameHash == other.NameHash && iParam == other.iParam;
        }

        public override bool Equals(object obj)
        {
            return obj is CarPartAttribute other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)NameHash * 397) ^ iParam;
            }
        }
    }
}
