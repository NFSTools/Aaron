using System.Collections.Generic;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services
{
    /// <summary>
    /// Service in charge of managing <see cref="Aaron.Data.AaronPresetCar"/> instances.
    /// </summary>
    public interface IPresetCarService : IChangeableManager
    {
        /// <summary>
        /// Fetch the list of preset cars.
        /// </summary>
        /// <returns>The list of preset cars.</returns>
        List<AaronPresetCar> GetPresetCars();

        /// <summary>
        /// Adds a preset car to the list.
        /// </summary>
        /// <param name="presetCar">The preset car to be added.</param>
        void AddPresetCar(AaronPresetCar presetCar);

        /// <summary>
        /// Finds a preset car by name.
        /// </summary>
        /// <param name="name">The name of the preset car to search for.</param>
        /// <returns>The preset car.</returns>
        AaronPresetCar FindPresetCar(string name);

        /// <summary>
        /// Resets the preset car service.
        /// </summary>
        void Reset();
    }
}