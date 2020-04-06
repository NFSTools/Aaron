using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services.Implementations
{
    public class PresetSkinService : IPresetSkinService
    {
        private readonly List<AaronPresetSkinRecord> _presetSkinRecords = new List<AaronPresetSkinRecord>();

        public List<AaronPresetSkinRecord> GetPresetSkins()
        {
            return _presetSkinRecords;
        }

        public void AddPresetSkin(AaronPresetSkinRecord presetSkin)
        {
            if (_presetSkinRecords.Contains(presetSkin))
            {
                throw new Exception("Attempted to add the same preset skin twice");
            }

            _presetSkinRecords.Add(presetSkin);
        }

        public AaronPresetSkinRecord FindPresetSkinByName(string name)
        {
            foreach (var aaronPresetSkinRecord in _presetSkinRecords)
            {
                if (string.Equals(aaronPresetSkinRecord.PresetName, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return aaronPresetSkinRecord;
                }
            }

            return null;
        }

        public void Reset()
        {
            _presetSkinRecords.Clear();
        }

        public bool HasAnyChanges()
        {
            return _presetSkinRecords.Any(ps => ps.Changed);
        }

        public List<ITracksChanges> GetChangedItems()
        {
            return _presetSkinRecords.Where(ps => ps.Changed).Cast<ITracksChanges>().ToList();
        }
    }
}
