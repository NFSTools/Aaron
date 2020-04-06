using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aaron.Data;
using Aaron.Enums;
using Aaron.Utils;

namespace Aaron.Services
{
    /// <summary>
    /// Interface describing a car service, responsible for managing car information records.
    /// </summary>
    public interface ICarService : IChangeableManager
    {
        /// <summary>
        /// Retrieves the list of car records.
        /// </summary>
        /// <returns>The list of car records.</returns>
        SynchronizedObservableCollection<AaronCarRecord> GetCars();

        /// <summary>
        /// Retrieves the list of car records of the given type.
        /// </summary>
        /// <param name="usageType">The car type to filter on.</param>
        /// <returns>The filtered list of car records.</returns>
        List<AaronCarRecord> GetCarsByType(CarUsageType usageType);

        /// <summary>
        /// Retrieves a car record by name.
        /// </summary>
        /// <param name="name">The name of the car record to retrieve.</param>
        /// <returns>The retrieved car record.</returns>
        AaronCarRecord FindCarByName(string name);

        /// <summary>
        /// Retrieves a car record by hash.
        /// </summary>
        /// <param name="hash">The hash of the car record to retrieve.</param>
        /// <returns>The retrieved car record.</returns>
        AaronCarRecord FindCarByHash(uint hash);

        /// <summary>
        /// Retrieves a car record by collision hash.
        /// </summary>
        /// <param name="collisionHash">The collision hash of the car record to retrieve.</param>
        /// <returns>The retrieved car record.</returns>
        AaronCarRecord FindCarByCollisionHash(uint collisionHash);

        /// <summary>
        /// Retrieves the list of car records from the given manufacturer.
        /// </summary>
        /// <param name="manufacturer">The manufacturer to filter on.</param>
        /// <returns>The filtered list of car records.</returns>
        List<AaronCarRecord> FindCarsByManufacturer(string manufacturer);

        /// <summary>
        /// Adds a car record to the list of car records.
        /// </summary>
        /// <param name="aaronCarRecord">The car record to be added.</param>
        void AddCar(AaronCarRecord aaronCarRecord);

        /// <summary>
        /// Resets the state of the service.
        /// </summary>
        void Reset();
    }
}