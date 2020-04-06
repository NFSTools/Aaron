using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Documents;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    [JsonObject]
    public class AaronDataTableEntry : ITracksChanges
    {
        /// <summary>
        /// The name of the table entry.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Unknown1
        /// </summary>
        public uint Unknown { get; set; }

        /// <summary>
        /// Unknown2
        /// </summary>
        public float Unknown2 { get; set; }

        [JsonIgnore]
        public bool Changed { get; set; }

        [JsonIgnore]
        public uint Hash => Name.StartsWith("0x")
            ? uint.Parse(Name.Substring(2), NumberStyles.AllowHexSpecifier)
            : Hashing.BinHash(Name);
    }

    [JsonObject]
    public class AaronDataTable : IEquatable<AaronDataTable>, ITracksChanges
    {
        [JsonProperty]
        public List<AaronDataTableEntry> Entries { get; set; }

        [JsonProperty]
        public string Name { get; set; }

        [JsonIgnore]
        public uint Hash => Name.StartsWith("0x")
            ? uint.Parse(Name.Substring(2), NumberStyles.AllowHexSpecifier)
            : Hashing.BinHash(Name);

        public bool Equals(AaronDataTable other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AaronDataTable)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(AaronDataTable left, AaronDataTable right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AaronDataTable left, AaronDataTable right)
        {
            return !Equals(left, right);
        }

        [JsonIgnore]
        public bool Changed { get; set; }
    }
}