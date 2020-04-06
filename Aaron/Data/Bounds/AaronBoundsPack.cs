using System.Collections.Generic;
using Newtonsoft.Json;

namespace Aaron.Data.Bounds
{
    /// <summary>
    /// Represents a collision bounds pack.
    /// </summary>
    public class AaronBoundsPack
    {
        /// <summary>
        /// The Jenkins hash of the pack's car's type name.
        /// </summary>
        [JsonIgnore]
        public uint NameHash { get; set; }

        /// <summary>
        /// The list of entries in the pack.
        /// </summary>
        public List<AaronBoundsEntry> Entries { get; set; }

        /// <summary>
        /// The list of point clouds in the pack.
        /// </summary>
        public List<AaronBoundsPointCloud> PointClouds { get; set; }

        /// <summary>
        /// Accesses the bounds entry at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AaronBoundsEntry this[int index]
        {
            get => Entries[index];
            set => Entries[index] = value;
        }
    }
}
