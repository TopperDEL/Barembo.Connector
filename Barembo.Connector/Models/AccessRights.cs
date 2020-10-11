using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// The access rights for a book in a bookshelf.
    /// </summary>
    public class AccessRights
    {
        /// <summary>
        /// Can read owner book entries
        /// </summary>
        public bool CanReadOwner { get; set; }

        /// <summary>
        /// Can read foreign book entries
        /// </summary>
        public bool CanReadForeign { get; set; }

        /// <summary>
        /// Can add book entries
        /// </summary>
        public bool CanAdd { get; set; }

        /// <summary>
        /// Can edit foreign entries
        /// </summary>
        public bool CanEditForeign { get; set; }

        /// <summary>
        /// Can edit own entries
        /// </summary>
        public bool CanEditOwn { get; set; }

        /// <summary>
        /// Can delete own entries
        /// </summary>
        public bool CanDeleteOwn { get; set; }

        /// <summary>
        /// Can delete foreign entries
        /// </summary>
        public bool CanDeleteForeign { get; set; }
    }
}
