using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Structures
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct bVector3
    {
        public float X;
        public float Y;
        public float Z;
        public float Pad;
    }
}
