using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data.Bounds;
using Aaron.Enums;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    /// <summary>
    /// A car record.
    /// </summary>
    [JsonObject]
    public class AaronCarRecord : IEquatable<AaronCarRecord>, ITracksChanges
    {
        [JsonProperty]
        public string CarTypeName { get; set; }
        [JsonProperty]
        public string BaseModelName { get; set; }
        [JsonProperty]
        public string ManufacturerName { get; set; }
        [JsonProperty]
        public CarUsageType UsageType { get; set; }
        [JsonProperty]
        public uint DefaultBasePaint { get; set; }
        [JsonProperty]
        public bool Skinnable { get; set; }
        [JsonProperty]
        public byte DefaultSkinNumber { get; set; }
        [JsonProperty]
        public AaronBoundsPack BoundsPack { get; set; }

        [JsonProperty]
        public AaronSpoilerRecord Spoiler { get; set; }

        [JsonIgnore]
        public uint PhysicsProfileHash
        {
            get { return Hashing.JenkinsHash(CarTypeName.ToLowerInvariant()); }
        }

        [JsonIgnore]
        public uint BaseCarID
        {
            get { return Hashing.BinHash(CarTypeName.ToUpperInvariant()); }
        }

        [JsonIgnore]
        public uint CollisionHash
        {
            get { return Hashing.JenkinsHash(CarTypeName.ToUpperInvariant()); }
        }

        public bool Equals(AaronCarRecord other)
        {
            if (other is null) return false;
            return ReferenceEquals(this, other) || string.Equals(CarTypeName, other.CarTypeName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((AaronCarRecord) obj);
        }

        public override int GetHashCode()
        {
            return CarTypeName.GetHashCode();
        }

        public static bool operator ==(AaronCarRecord left, AaronCarRecord right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AaronCarRecord left, AaronCarRecord right)
        {
            return !Equals(left, right);
        }

        [JsonIgnore]
        public bool Changed { get; set; }
    }
}
