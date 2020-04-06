using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Utils
{
    public static class VersionReader
    {
        public static string AppVersion = "";

        static VersionReader()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyMetadataAttribute), false);

            foreach (var attribute in attributes)
            {
                if (attribute is AssemblyMetadataAttribute ama && ama.Key == "GitHash")
                {
                    AppVersion = ama.Value;
                }
            }
        }
    }
}
