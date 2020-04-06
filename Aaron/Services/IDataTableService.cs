using System.Collections.Generic;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services
{
    /// <summary>
    /// Service for managing <see cref="Aaron.Data.AaronDataTable"/> instances
    /// </summary>
    public interface IDataTableService : IChangeableManager
    {
        /// <summary>
        /// Fetch the list of data tables.
        /// </summary>
        /// <returns>The list of data tables.</returns>
        List<AaronDataTable> GetDataTables();

        /// <summary>
        /// Add a data table.
        /// </summary>
        /// <param name="dataTable">The data table to add.</param>
        void AddDataTable(AaronDataTable dataTable);

        /// <summary>
        /// Reset the data table service.
        /// </summary>
        void Reset();
    }
}