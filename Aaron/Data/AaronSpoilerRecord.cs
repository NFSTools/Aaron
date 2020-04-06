using System.ComponentModel;
using System.Runtime.Serialization;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    public enum AaronSpoilerType : uint
    {
        [EnumMember]
        None = 0x5C67B1EC,
        [EnumMember]
        Small = 0xE9B71B15,
        [EnumMember]
        Large = 0xE93222A7,
        [EnumMember]
        Hatch = 0xE8E9C8A4
    }
    
    [JsonObject]
    public class AaronSpoilerRecord
    {
        [JsonProperty]
        public AaronSpoilerType SpoilerType { get; set; }

        [JsonIgnore]
        public string SpoilerName => $"SPOILER_{SpoilerType.ToString().ToUpperInvariant()}";

        [JsonIgnore]
        public uint CarTypeNameHash { get; set; }
    }
}
