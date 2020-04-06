using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aaron.Data;
using GalaSoft.MvvmLight.Messaging;

namespace Aaron.Messages
{
    /// <summary>
    /// Message sent when the active project is changed.
    /// </summary>
    public class ProjectMessage : MessageBase
    {
        /// <summary>
        /// The new active project.
        /// </summary>
        public AaronProject Project { get; }

        public ProjectMessage(AaronProject project)
        {
            Project = project;
        }
    }
}
