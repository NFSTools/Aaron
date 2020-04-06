using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Data.Bounds
{
    [Flags]
    public enum AaronBoundsFlags : ushort
    {
        kBounds_Disabled = 0x1,
        kBounds_PrimVsWorld = 0x2,
        kBounds_PrimVsObjects = 0x4,
        kBounds_PrimVsGround = 0x8,
        kBounds_MeshVsGround = 0x10,
        kBounds_Internal = 0x20,
        kBounds_Box = 0x40,
        kBounds_Sphere = 0x80,
        kBounds_Constraint_Conical = 0x100,
        kBounds_Constraint_Prismatic = 0x200,
        kBounds_Joint_Female = 0x400,
        kBounds_Joint_Male = 0x800,
        kBounds_Male_Post = 0x1000,
        kBounds_Joint_Invert = 0x2000,
        kBounds_PrimVsOwnParts = 0x4000,
    }
}
