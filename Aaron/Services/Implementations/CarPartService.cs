using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services.Implementations
{
    public class CarPartService : ICarPartService
    {
        private readonly SynchronizedObservableCollection<AaronCarPartCollection> _carPartCollections;
        private readonly List<string> _strings;
        private byte[] _slotOverrideData;

        public CarPartService()
        {
            _carPartCollections = new SynchronizedObservableCollection<AaronCarPartCollection>();
            _strings = new List<string>();
        }

        public SynchronizedObservableCollection<AaronCarPartCollection> GetCarPartCollections()
        {
            return _carPartCollections;
        }

        public AaronCarPartCollection GetCarPartCollectionByIndex(int index)
        {
            if (index >= _carPartCollections.Count)
            {
                throw new IndexOutOfRangeException("Attempted to access car part collection at index " + index);
            }

            return _carPartCollections[index];
        }

        public AaronCarPartCollection this[int index] => GetCarPartCollectionByIndex(index);

        public AaronCarPartCollection this[uint hash] => GetCarPartCollectionByHash(hash);

        public AaronCarPartCollection GetCarPartCollectionByHash(uint hash)
        {
            return _carPartCollections.First(c => c.Hash == hash);
        }

        public AaronCarPartCollection GetCarPartCollectionByName(string name)
        {
            foreach (var aaronCarPartCollection in _carPartCollections)
            {
                if (aaronCarPartCollection.Name == name)
                    return aaronCarPartCollection;
            }

            return null;
            //return _carPartCollections.First(c => c.Name == name);
        }

        public void AddCarPartCollection(AaronCarPartCollection collection)
        {
            if (_carPartCollections.Contains(collection))
                throw new ArgumentException("Attempted to add a duplicate car part collection");
            _carPartCollections.Add(collection);
        }

        public AaronCarPartCollection ReplaceCarPartCollection(uint hash, AaronCarPartCollection collection)
        {
            var index = _carPartCollections.IndexOf(GetCarPartCollectionByHash(hash));

            return _carPartCollections[index] = collection;
        }

        public byte[] GetSlotOverrideData()
        {
            return _slotOverrideData;
        }

        public void SetSlotOverrideData(byte[] data)
        {
            _slotOverrideData = data;
        }

        public List<string> GetStrings()
        {
            return _strings;
        }

        public void AddString(string str)
        {
            if (!_strings.Contains(str))
                _strings.Add(str);
        }

        public void Reset()
        {
            _carPartCollections.Clear();
            _strings.Clear();
            _slotOverrideData = null;
        }

        public bool HasAnyChanges()
        {
            return _carPartCollections.Any(c => c.Changed);
        }

        public List<ITracksChanges> GetChangedItems()
        {
            return _carPartCollections.Where(c => c.Changed).Cast<ITracksChanges>().ToList();
        }
    }
}
