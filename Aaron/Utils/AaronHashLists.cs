using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Utils
{
    public static class AaronHashLists
    {
        private static readonly Dictionary<string, List<string>> _hashLists = new Dictionary<string, List<string>>();

        public static void Load(string name, string filePath)
        {
            Debug.WriteLine("AaronHashLists: loading list '{0}' at '{1}'", name, filePath);
            _hashLists[name] = new List<string>(File.ReadAllLines(filePath).SkipWhile(l => l.StartsWith("# ")));
            Debug.WriteLine("AaronHashLists: loaded {0} hashes into '{1}'", _hashLists[name].Count, name);
        }

        public static List<string> Get(string name)
        {
            return _hashLists[name];
        }
    }
}
