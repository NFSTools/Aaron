using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Aaron.Enums;
using Aaron.Structures;
using Aaron.Utils;
using Newtonsoft.Json;

namespace Aaron.Data
{
    [JsonObject]
    public class AaronCarPartAttribute
    {
        /// <summary>
        /// The resolved name of the attribute
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// The value of the attribute
        /// </summary>
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// The strings the attribute references
        /// </summary>
        public List<string> Strings { get; } = new List<string>();

        [JsonIgnore]
        public uint Hash => Name.StartsWith("0x")
            ? uint.Parse(Name.Substring(2), NumberStyles.AllowHexSpecifier)
            : Hashing.BinHash(Name);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 0;
                hash ^= (int)(Hash * 397);
                hash ^= (int)(GetValueHashCode() * 397);
                hash ^= (int)(GetStringsHashCode() * 397);

                return hash;
            }
        }

        private int GetStringsHashCode()
        {
            int res = 0x2D2816FE;
            foreach (var item in Strings)
            {
                res = res * 31 + (item?.GetHashCode() ?? 0);
            }
            return res;
        }

        private int GetValueHashCode()
        {
            if (Value is double d)
            {
                byte[] data = BitConverter.GetBytes(d);
                int x = BitConverter.ToInt32(data, 0);
                int y = BitConverter.ToInt32(data, 4);
                return x ^ y;
            }

            if (Value is float f)
            {
                return BitConverter.ToInt32(BitConverter.GetBytes(f), 0);
            }

            if (Value is CarPartID cpi)
            {
                return (int)cpi;
            }

            return Value?.GetHashCode() ?? 0;
        }
    }
}
