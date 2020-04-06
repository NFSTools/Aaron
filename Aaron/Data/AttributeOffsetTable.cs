using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Data
{
    public class AttributeOffsetTable
    {
        public long Offset { get; set; }

        public List<ushort> Offsets { get; set; }
    }
}
