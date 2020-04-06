using System;

namespace Aaron.Services
{
    public interface IDataService
    {
        /// <summary>
        /// Loads the car controller file at the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="progress"></param>
        void LoadCarController(string path, IProgress<string> progress = null);
    }
}