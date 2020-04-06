using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services.Implementations
{
    public class PresetCarService : IPresetCarService
    {
        private readonly List<AaronPresetCar> _presetCars = new List<AaronPresetCar>();

        public List<AaronPresetCar> GetPresetCars()
        {
            return _presetCars;
        }

        public void AddPresetCar(AaronPresetCar presetCar)
        {
            if (_presetCars.Contains(presetCar))
                throw new DuplicateNameException("Attempted to add the same preset car twice");

            _presetCars.Add(presetCar);
        }

        public AaronPresetCar FindPresetCar(string name)
        {
            foreach (var aaronPresetCar in _presetCars)
            {
                if (string.Equals(aaronPresetCar.PresetName, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return aaronPresetCar;
                }
            }

            return null;
        }

        public void Reset()
        {
            _presetCars.Clear();
        }

        public bool HasAnyChanges()
        {
            return _presetCars.Any(pc => pc.Changed);
        }

        public List<ITracksChanges> GetChangedItems()
        {
            return _presetCars.Where(c => c.Changed).Cast<ITracksChanges>().ToList();
        }
    }
}
