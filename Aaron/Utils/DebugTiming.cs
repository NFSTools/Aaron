using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aaron.Utils
{
    public static class DebugTiming
    {
        private static Dictionary<string, Stopwatch> timingDictionary = new Dictionary<string, Stopwatch>();

        public static void BeginTiming(string name)
        {
#if DEBUG
            Debug.WriteLine("BeginTiming {0}", new[] { name });
            timingDictionary[name] = new Stopwatch();
            timingDictionary[name].Start();
#endif
        }

        public static void EndTiming(string name)
        {
#if DEBUG
            timingDictionary[name].Stop();
            Debug.WriteLine("EndTiming {0} - {1}ms", new object[] { name, timingDictionary[name].ElapsedMilliseconds });
#endif
        }
    }
}
