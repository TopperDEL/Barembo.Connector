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
        /// Can read all book entries
        /// </summary>
        public bool CanReadEntries { get; set; }

        /// <summary>
        /// Can read foreign book entries - if false, means only own entries can be read
        /// </summary>
        public bool CanReadForeignEntries { get; set; }

        /// <summary>
        /// Can add book entries
        /// </summary>
        public bool CanAddEntries { get; set; }

        /// <summary>
        /// Can edit foreign entries
        /// </summary>
        public bool CanEditForeignEntries { get; set; }

        /// <summary>
        /// Can edit own entries
        /// </summary>
        public bool CanEditOwnEntries { get; set; }

        /// <summary>
        /// Can delete all entries
        /// </summary>
        public bool CanDeleteEntries { get; set; }

        /// <summary>
        /// Can delete foreign entries
        /// </summary>
        public bool CanDeleteForeignEntries { get; set; }

        /// <summary>
        /// Can share the Book with others
        /// </summary>
        public bool CanShareBook { get; set; }

        /// <summary>
        /// Can edit the Book
        /// </summary>
        public bool CanEditBook { get; set; }

        public static AccessRights Full
        {
            get
            {
                return new AccessRights()
                {
                    CanAddEntries = true,
                    CanDeleteForeignEntries = true,
                    CanDeleteEntries = true,
                    CanEditForeignEntries = true,
                    CanEditOwnEntries = true,
                    CanReadForeignEntries = true,
                    CanReadEntries = true,
                    CanShareBook = true,
                    CanEditBook = true
                };
            }
        }
    }
}
