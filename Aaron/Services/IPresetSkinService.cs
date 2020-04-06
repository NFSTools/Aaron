using System.Collections.Generic;
using System.Windows.Documents;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services
{
    /// <summary>
    /// Service for managing <see cref="Aaron.Data.AaronPresetSkinRecord"/> instances
    /// </summary>
    public interface IPresetSkinService : IChangeableManager
    {
        /// <summary>
        /// Fetch the list of preset skins.
        /// </summary>
        /// <returns>The list of preset skins.</returns>
        List<AaronPresetSkinRecord> GetPresetSkins();

        /// <summary>
        /// Add a preset skin.
        /// </summary>
        /// <param name="presetSkin">The preset skin to add.</param>
        void AddPresetSkin(AaronPresetSkinRecord presetSkin);

        /// <summary>
        /// Find the preset skin with the given name.
        /// </summary>
        /// <param name="name">The name of the preset skin to find.</param>
        /// <returns></returns>
        AaronPresetSkinRecord FindPresetSkinByName(string name);

        /// <summary>
        /// Reset the preset skin service.
        /// </summary>
        void Reset();
    }
}