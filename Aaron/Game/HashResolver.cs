using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Game
{
    /// <summary>
    /// Manages hash-to-text mappings.
    /// </summary>
    public static class HashResolver
    {
        private static readonly Dictionary<uint, string> HashDictionary = new Dictionary<uint, string>();

        public static string Resolve(uint hash)
        {
            return HashDictionary.TryGetValue(hash, out var s) ? s : $"0x{hash:X8}";
        }

        public static void Add(uint hash, string text)
        {
            HashDictionary[hash] = text;
        }
    }
}
