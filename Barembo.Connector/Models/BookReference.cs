using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A reference to a book in a bookshelf.
    /// </summary>
    public class BookReference
    {
        /// <summary>
        /// The ID of a book.
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// The name of the owner of the book.
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The access grant to access that book.
        /// </summary>
        public string AccessGrant { get; set; }

        /// <summary>
        /// The access rights to access that book.
        /// </summary>
        public AccessRights AccessRights { get; set; }
    }
}
