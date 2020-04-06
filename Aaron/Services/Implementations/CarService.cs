using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Enums;
using Aaron.Utils;

namespace Aaron.Services.Implementations
{
    public class CarService : ICarService
    {
        private readonly SynchronizedObservableCollection<AaronCarRecord> _carRecords;

        public CarService()
        {
            _carRecords = new SynchronizedObservableCollection<AaronCarRecord>();
        }

        public SynchronizedObservableCollection<AaronCarRecord> GetCars()
        {
            return _carRecords;
        }

        public List<AaronCarRecord> GetCarsByType(CarUsageType usageType)
        {
            return _carRecords.Where(c => c.UsageType == usageType).ToList();
        }

        public AaronCarRecord FindCarByName(string name)
        {
            foreach (var aaronCarRecord in _carRecords)
            {
                if (string.Equals(aaronCarRecord.CarTypeName, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return aaronCarRecord;
                }
            }

            return null;
        }

        public AaronCarRecord FindCarByHash(uint hash)
        {
            foreach (var aaronCarRecord in _carRecords)
            {
                if (aaronCarRecord.BaseCarID == hash)
                    return aaronCarRecord;
            }

            return null;
        }

        public AaronCarRecord FindCarByCollisionHash(uint collisionHash)
        {
            foreach (var aaronCarRecord in _carRecords)
            {
                if (aaronCarRecord.CollisionHash == collisionHash)
                    return aaronCarRecord;
            }

            return null;
        }

        public List<AaronCarRecord> FindCarsByManufacturer(string manufacturer)
        {
            throw new NotImplementedException();
        }

        public void AddCar(AaronCarRecord aaronCarRecord)
        {
            if (_carRecords.Contains(aaronCarRecord))
            {
                throw new ArgumentException("Attempted to add a car record that is already known", nameof(aaronCarRecord));
            }

            _carRecords.Add(aaronCarRecord);
        }

        public void Reset()
        {
            _carRecords.Clear();
        }

        public bool HasAnyChanges()
        {
            return _carRecords.Any(c => c.Changed);
        }

        public List<ITracksChanges> GetChangedItems()
        {
            return _carRecords.Where(c => c.Changed).Cast<ITracksChanges>().ToList();
        }
    }
}
