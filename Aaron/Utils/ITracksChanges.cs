using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Utils
{
    public interface ITracksChanges
    {
        bool Changed { get; set; }
    }
}
