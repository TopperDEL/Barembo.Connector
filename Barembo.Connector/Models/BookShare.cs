using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A BookShare contains all information to access a foreign book. It gets written
    /// to the own store and only the BookShareReference gets shared.
    /// </summary>
    public class BookShare
    {
        /// <summary>
        /// The Id of that BookShare
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The Id of the shared Book
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// The owner name of the shared Book
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// The StoreAccess to this Book
        /// </summary>
        public StoreAccess Access { get; set; }

        /// <summary>
        /// The AccessRights for this Book
        /// </summary>
        public AccessRights AccessRights { get; set; }
    }
}
