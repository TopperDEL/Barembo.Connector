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
        /// The ID of the contributor.
        /// </summary>
        public string ContributorId { get; set; }

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

        /// <summary>
        /// If the Book is a foreign Book the BookShareReference holds the reference
        /// to the BookShare to use for this Book. It gets refreshed before every access.
        /// </summary>
        public BookShareReference BookShareReference { get; set; }

        internal BookReference() { }
    }
}
