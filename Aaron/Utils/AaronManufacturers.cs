using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Aaron.Utils
{
    public static class AaronManufacturers
    {
        private static readonly List<string> Manufacturers = new List<string>();

        public static void LoadFromFile(string file)
        {
            foreach (var line in File.ReadAllLines(file).Where(l => !l.StartsWith("#") && !string.IsNullOrWhiteSpace(l)))
            {
                Manufacturers.Add(line.Trim());
            }

            Manufacturers.Sort();
        }

        public static List<string> GetManufacturers()
        {
            return Manufacturers;
        }
    }
}