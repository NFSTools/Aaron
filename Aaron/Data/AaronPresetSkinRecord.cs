using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    /// <summary>
    /// Represents a preset skin entry.
    /// </summary>
    [JsonObject]
    public class AaronPresetSkinRecord : IEquatable<AaronPresetSkinRecord>, ITracksChanges
    {
        /// <summary>
        /// The preset skin's name.
        /// </summary>
        [JsonProperty]
        public string PresetName { get; set; }

        /// <summary>
        /// The paint group hash.
        /// </summary>
        [JsonProperty]
        public uint PaintGroup { get; set; }

        /// <summary>
        /// The paint hue hash.
        /// </summary>
        [JsonProperty]
        public uint PaintHue { get; set; }

        /// <summary>
        /// The saturation of the paint.
        /// </summary>
        [JsonProperty]
        public byte Saturation { get; set; }

        /// <summary>
        /// The variance of the paint.
        /// </summary>
        [JsonProperty]
        public byte Variance { get; set; }

        public bool Equals(AaronPresetSkinRecord other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return PresetName == other.PresetName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AaronPresetSkinRecord) obj);
        }

        public override int GetHashCode()
        {
            return PresetName.GetHashCode();
        }

        public static bool operator ==(AaronPresetSkinRecord left, AaronPresetSkinRecord right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AaronPresetSkinRecord left, AaronPresetSkinRecord right)
        {
            return !Equals(left, right);
        }

        [JsonIgnore]
        public bool Changed { get; set; }
    }
}
