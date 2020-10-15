using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// A BookShareReference holds the info to access a BookShare on the store.
    /// The BookShare holds the info about the shared Book itself, which might be
    /// much more data.
    /// The BookShareReference shall be as small as possible to be shareable as QR-code.
    /// </summary>
    public class BookShareReference
    {
        /// <summary>
        /// The StoreAccess to the BookShareKey. It should be a read-only
        /// access to the BookShare-object.
        /// </summary>
        public StoreAccess StoreAccess { get; set; }

        /// <summary>
        /// The key to the BookShare
        /// </summary>
        public StoreKey StoreKey { get; set; }

        internal BookShareReference() { }
    }
}
