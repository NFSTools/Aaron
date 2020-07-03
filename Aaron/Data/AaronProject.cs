using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Aaron.Data
{
    /// <summary>
    /// A project descriptor
    /// </summary>
    [JsonObject]
    public class AaronProject
    {
        [JsonIgnore]
        public string Path { get; set; }
        [JsonIgnore]
        public string Directory { get; set; }

        [JsonProperty]
        public uint Version { get; set; }

        [JsonProperty]
        public string CarsDirectory { get; set; }

        [JsonProperty]
        public string CarPartsDirectory { get; set; }

        [JsonProperty]
        public string PresetCarsDirectory { get; set; }

        [JsonProperty]
        public string PresetSkinsDirectory { get; set; }

        [JsonProperty]
        public string DataTablesDirectory { get; set; }

        [JsonProperty]
        public string OutputFilePath { get; set; }

        // TODO: get rid of this! or at least make it editable
        [JsonProperty]
        public byte[] SlotOverrideData { get; set; }
    }
}
