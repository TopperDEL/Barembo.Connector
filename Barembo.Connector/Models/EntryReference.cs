using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// An EntryReference holds information about an Entry, where it belongs to and where it can be found
    /// </summary>
    public struct EntryReference : IEquatable<EntryReference>
    {
        /// <summary>
        /// Too BookReference to the Book this Entry belongs to
        /// </summary>
        public BookReference BookReference { get; set; }

        /// <summary>
        /// The Key of the Entry in the store
        /// </summary>
        public string EntryKey { get; set; }

        /// <summary>
        /// The Id of the Entry referenced
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// The creation date of that entry - to be used to sort entries
        /// </summary>
        public DateTime CreationDate { get; set; }

        public bool Equals(EntryReference other)
        {
            return BookReference.BookId == other.BookReference.BookId && EntryId == other.EntryId;
        }
    }
}
