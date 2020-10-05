using Barembo.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// The list of possible StoreKey-Types
    /// </summary>
    public enum StoreKeyTypes
    {
        /// <summary>
        /// The BookShelf of a user
        /// </summary>
        BookShelf = 1,
        /// <summary>
        /// A Book
        /// </summary>
        Book = 2
    }

    /// <summary>
    /// A StoreKey is the definition of an object in the store. It helps
    /// to generate the key to locate the object within the store.
    /// </summary>
    public class StoreKey
    {
        /// <summary>
        /// The type of this StoreKey
        /// </summary>
        public StoreKeyTypes StoreKeyType { get; private set; }

        /// <summary>
        /// Properties to further define this StoreKey
        /// </summary>
        public Dictionary<string, string> Properties { get; private set; }

        /// <summary>
        /// Creates a StoreKey
        /// </summary>
        /// <param name="storeKeyType">The type of this StoreKey</param>
        /// <param name="properties">The parameters - if necessary - to further define this StoreKey</param>
        public StoreKey(StoreKeyTypes storeKeyType, Dictionary<string, string> properties = null)
        {
            StoreKeyType = storeKeyType;
            Properties = properties;
        }

        /// <summary>
        /// Returns the StoreKey for a BookShelf
        /// </summary>
        /// <returns>The StoreKey</returns>
        public static StoreKey BookShelf()
        {
            return new StoreKey(StoreKeyTypes.BookShelf );
        }

        /// <summary>
        /// Returns the StoreKey for a Book
        /// </summary>
        /// <returns>The StoreKey</returns>
        public static StoreKey Book(string bookId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);

            return new StoreKey(StoreKeyTypes.Book, properties);
        }

        /// <summary>
        /// Gets the StoreKey as a string using the StoreKeyHelper
        /// </summary>
        /// <returns>The StoreKey as a string</returns>
        public override string ToString()
        {
            return StoreKeyHelper.Convert(this);
        }
    }
}
