using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services
{
    public interface ICarPartService : IChangeableManager
    {
        #region Car Part Collections

        SynchronizedObservableCollection<AaronCarPartCollection> GetCarPartCollections();

        AaronCarPartCollection GetCarPartCollectionByIndex(int index);

        AaronCarPartCollection this[int index] { get; }
        AaronCarPartCollection this[uint hash] { get; }

        AaronCarPartCollection GetCarPartCollectionByHash(uint hash);
        AaronCarPartCollection GetCarPartCollectionByName(string name);

        void AddCarPartCollection(AaronCarPartCollection collection);
        AaronCarPartCollection ReplaceCarPartCollection(uint hash, AaronCarPartCollection collection);

        #endregion

        byte[] GetSlotOverrideData();
        void SetSlotOverrideData(byte[] data);

        List<string> GetStrings();
        void AddString(string str);

        void Reset();
    }
}
