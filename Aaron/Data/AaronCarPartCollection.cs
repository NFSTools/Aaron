using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    /// <summary>
    /// A collection of car part records.
    /// </summary>
    public class AaronCarPartCollection : ITracksChanges
    {
        /// <summary>
        /// The name of the collection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The parts in the collection.
        /// </summary>
        public SynchronizedObservableCollection<AaronCarPartRecord> Parts { get; set; }

        /// <summary>
        /// The hash of the collection's name.
        /// </summary>
        [JsonIgnore]
        public uint Hash => Name.StartsWith("0x")
            ? uint.Parse(Name.Substring(2), NumberStyles.AllowHexSpecifier)
            : Hashing.BinHash(Name);

        [JsonIgnore]
        public bool Changed { get; set; }
    }
}
