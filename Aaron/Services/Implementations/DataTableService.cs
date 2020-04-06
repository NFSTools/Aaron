using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services.Implementations
{
    public class DataTableService : IDataTableService
    {
        private readonly List<AaronDataTable> _dataTables = new List<AaronDataTable>();

        public List<AaronDataTable> GetDataTables()
        {
            return _dataTables;
        }

        public void AddDataTable(AaronDataTable dataTable)
        {
            if (_dataTables.Contains(dataTable))
                throw new Exception("Attempted to add the same data table twice");
            _dataTables.Add(dataTable);
        }

        public void Reset()
        {
            _dataTables.Clear();
        }

        public bool HasAnyChanges()
        {
            return _dataTables.Any(d => d.Changed);
        }

        public List<ITracksChanges> GetChangedItems()
        {
            return _dataTables.Where(c => c.Changed).Cast<ITracksChanges>().ToList();
        }
    }
}
