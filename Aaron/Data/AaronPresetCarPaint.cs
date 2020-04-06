namespace Aaron.Data
{
    /// <summary>
    /// Represents a paint entry on a preset car.
    /// </summary>
    public class AaronPresetCarPaint
    {
        /// <summary>
        /// The item hash of the paint group.
        /// </summary>
        public uint Group { get; set; }

        /// <summary>
        /// The item hash of the paint hue.
        /// </summary>
        public uint Hue { get; set; }

        /// <summary>
        /// The saturation of the paint.
        /// </summary>
        public byte Saturation { get; set; }

        /// <summary>
        /// The variance of the paint.
        /// </summary>
        public byte Variance { get; set; }
    }
}
