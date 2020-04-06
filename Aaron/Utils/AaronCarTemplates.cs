using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;

namespace Aaron.Utils
{
    public static class AaronCarTemplates
    {
        private static Dictionary<string, List<AaronCarPartRecord>> _templateDictionary = new Dictionary<string, List<AaronCarPartRecord>>();

        public static void LoadFromFile(string name, string file)
        {
            _templateDictionary[name] =
                Serialization.Deserialize<AaronCarPartCollection>(File.ReadAllText(file)).Parts.ToList();
        }

        public static List<AaronCarPartRecord> Get(string name)
        {
            return _templateDictionary[name];
        }
    }
}
