using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    /// <summary>
    /// Represents a preset car entry.
    /// </summary>
    [DataContract(Name = "PresetCar", Namespace = "Aaron.Data.Presets")]
    public class AaronPresetCar : ITracksChanges
    {
        /// <summary>
        /// The VLT collection name of the car being used.
        /// </summary>
        /// <example>supra</example>
        [DataMember]
        public string CarName { get; set; }

        /// <summary>
        /// The commerce DB name of the preset.
        /// </summary>
        /// <example>supra_DRAG_1</example>
        [DataMember]
        public string PresetName { get; set; }

        /// <summary>
        /// The installed visual parts. 123 slots are available.
        /// </summary>
        [DataMember]
        public SynchronizedObservableCollection<AaronPresetCarVisualPart> VisualParts { get; set; }

        /// <summary>
        /// The paints applied to the preset.
        /// </summary>
        [DataMember]
        public SynchronizedObservableCollection<AaronPresetCarPaint> Paints { get; set; }

        /// <summary>
        /// The installed performance parts. 6 slots are available.
        /// </summary>
        [DataMember]
        public SynchronizedObservableCollection<AaronPresetCarPerfPart> PerformanceParts { get; set; }

        /// <summary>
        /// The installed skill mods. 6 slots are available.
        /// </summary>
        [DataMember]
        public SynchronizedObservableCollection<AaronPresetCarSkill> SkillModParts { get; set; }

        /// <summary>
        /// The number of skill mod slots that this preset has.
        /// </summary>
        [DataMember]
        public uint SkillModSlotCount { get; set; }

        /// <summary>
        /// The vinyls applied to the preset.
        /// </summary>
        [DataMember]
        public SynchronizedObservableCollection<AaronPresetCarVinyl> Vinyls { get; set; }

        [JsonIgnore]
        public bool Changed { get; set; }
    }
}
