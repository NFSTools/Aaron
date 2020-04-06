namespace Aaron.Data
{
    /// <summary>
    /// Represents a hue of a vinyl.
    /// </summary>
    public class AaronPresetCarVinylHue
    {
        /// <summary>
        /// The item hash of the vinyl hue.
        /// </summary>
        public uint Hue { get; set; }

        /// <summary>
        /// The saturation of the vinyl.
        /// </summary>
        public byte Saturation { get; set; }

        /// <summary>
        /// The variance of the vinyl.
        /// </summary>
        public byte Variance { get; set; }
    }

    /// <summary>
    /// Represents a vinyl entry on a preset car.
    /// </summary>
    public class AaronPresetCarVinyl
    {
        /// <summary>
        /// The vinyl part hash.
        /// </summary>
        public uint Hash { get; set; }

        /// <summary>
        /// The X-translation of the vinyl.
        /// </summary>
        public short TranX { get; set; }

        /// <summary>
        /// The Y-translation of the vinyl.
        /// </summary>
        public short TranY { get; set; }

        /// <summary>
        /// The X-scale of the vinyl.
        /// </summary>
        public short ScaleX { get; set; }

        /// <summary>
        /// The Y-scale of the vinyl.
        /// </summary>
        public short ScaleY { get; set; }

        /// <summary>
        /// The rotation of the vinyl.
        /// </summary>
        public byte Rotation { get; set; }

        /// <summary>
        /// The shear of the vinyl.
        /// </summary>
        public byte Shear { get; set; }

        /// <summary>
        /// The vinyl's hues. 4 slots are available.
        /// </summary>
        public AaronPresetCarVinylHue[] Hues { get; set; }

        /// <summary>
        /// Whether or not the vinyl is mirrored. Mirrored vinyls appear identically on both sides of the car.
        /// </summary>
        public bool IsMirrored { get; set; }
    }
}
