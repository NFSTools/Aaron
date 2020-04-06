using System.Collections.Generic;
using Aaron.Data;
using Aaron.Utils;

namespace Aaron.Services
{
    /// <summary>
    /// Service responsible for managing Projects.
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// Generates project data based on the current loaded state.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>The path to the new project file.</returns>
        string GenerateProject(string directory);

        /// <summary>
        /// Saves the given project.
        /// </summary>
        void SaveProject();

        /// <summary>
        /// Loads project data.
        /// </summary>
        /// <param name="file">The full path to the project file.</param>
        void LoadProject(string file);

        /// <summary>
        /// Fetches the current project.
        /// </summary>
        /// <returns>The current project.</returns>
        AaronProject GetCurrentProject();

        /// <summary>
        /// Determines if there are any unsaved changes.
        /// </summary>
        /// <returns>Whether or not there are any unsaved changes.</returns>
        bool HasUnsavedChanges();

        List<ITracksChanges> GetChangedItems();

        /// <summary>
        /// Closes the current project and resets all internal state.
        /// </summary>
        void CloseProject();
    }
}