using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Utils;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace Aaron.Data
{
    /// <summary>
    /// A car part record.
    /// </summary>
    public class AaronCarPartRecord : ObservableObject, ITracksChanges
    {
        private string _name;

        /// <summary>
        /// The name of the car part.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
                RaisePropertyChanged("Hash");
            }
        }

        /// <summary>
        /// The list of part attributes.
        /// </summary>
        public SynchronizedObservableCollection<AaronCarPartAttribute> Attributes { get; set; }

        [JsonIgnore]
        public uint Hash => Name.StartsWith("0x")
            ? uint.Parse(Name.Substring(2), NumberStyles.AllowHexSpecifier)
            : Hashing.BinHash(Name);

        [JsonIgnore]
        public uint CollectionHash { get; set; }

        [JsonIgnore]
        public bool Changed { get; set; }
    }
}
