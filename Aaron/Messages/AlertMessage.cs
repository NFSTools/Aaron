using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Aaron.Messages
{
    public enum AlertType
    {
        Information,
        Warning,
        Error,
    }

    /// <summary>
    /// A message containing alert parameters.
    /// </summary>
    public class AlertMessage : MessageBase
    {
        /// <summary>
        /// The alert type.
        /// </summary>
        public AlertType Type { get; set; }

        /// <summary>
        /// The alert title.
        /// </summary>
        public string Title
        {
            get { return "Aaron"; }
        }

        /// <summary>
        /// The alert body.
        /// </summary>
        public string Body { get; set; }

        public AlertMessage(AlertType type, string body)
        {
            Type = type;
            Body = body;
        }
    }
}
