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
        Book = 2,
        /// <summary>
        /// An Entry
        /// </summary>
        Entry = 3,
        /// <summary>
        /// A list of entries
        /// </summary>
        Entries = 4,
        /// <summary>
        /// An attachment
        /// </summary>
        Attachment = 5,
        /// <summary>
        /// A contributor
        /// </summary>
        Contributor = 6,
        /// <summary>
        /// A list of contributors
        /// </summary>
        Contributors = 7,
        /// <summary>
        /// A BookShare
        /// </summary>
        BookShare = 8,
        /// <summary>
        /// A list of BookShares
        /// </summary>
        BookShares = 9,
        /// <summary>
        /// An attachment-preview
        /// </summary>
        AttachmentPreview = 10,
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
        public StoreKeyTypes StoreKeyType { get; set; }

        /// <summary>
        /// Properties to further define this StoreKey
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// Creates a StoreKey
        /// </summary>
        /// <param name="storeKeyType">The type of this StoreKey</param>
        /// <param name="properties">The parameters to further define this StoreKey</param>
        public StoreKey(StoreKeyTypes storeKeyType, Dictionary<string, string> properties)
        {
            StoreKeyType = storeKeyType;
            Properties = properties;
        }

        /// <summary>
        /// Creates a StoreKey
        /// </summary>
        /// <param name="storeKeyType">The type of this StoreKey</param>
        public StoreKey(StoreKeyTypes storeKeyType)
        {
            StoreKeyType = storeKeyType;
        }

        /// <summary>
        /// Returns the StoreKey for a BookShelf
        /// </summary>
        /// <returns>The StoreKey</returns>
        public static StoreKey BookShelf()
        {
            return new StoreKey(StoreKeyTypes.BookShelf);
        }

        /// <summary>
        /// Returns the StoreKey for a Book
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey Book(string bookId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);

            return new StoreKey(StoreKeyTypes.Book, properties);
        }

        /// <summary>
        /// Return the StoreKey for an Entry
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <param name="entryId">The Id of an Entry</param>
        /// <param name="contributorId">The Id of a Contributor</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey Entry(string bookId, string entryId, string contributorId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);
            properties.Add(StoreKeyHelper.PROPERTY_ENTRY_ID, entryId);
            properties.Add(StoreKeyHelper.PROPERTY_CONTRIBUTOR_ID, contributorId);

            return new StoreKey(StoreKeyTypes.Entry, properties);
        }

        /// <summary>
        /// Returns the StoreKey for a List of Entries
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey Entries(string bookId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);

            return new StoreKey(StoreKeyTypes.Entries, properties);
        }

        /// <summary>
        /// Returns the StoreKey for an Attachment
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <param name="entryId">The Id of an Entry</param>
        /// <param name="contributorId">The Id of a Contributor</param>
        /// <param name="attachmentId">The Id of an Attachment</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey Attachment(string bookId, string entryId, string contributorId, string attachmentId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);
            properties.Add(StoreKeyHelper.PROPERTY_ENTRY_ID, entryId);
            properties.Add(StoreKeyHelper.PROPERTY_CONTRIBUTOR_ID, contributorId);
            properties.Add(StoreKeyHelper.PROPERTY_ATTACHMENT_ID, attachmentId);

            return new StoreKey(StoreKeyTypes.Attachment, properties);
        }

        /// <summary>
        /// Returns the StoreKey for an Attachment-Preview
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <param name="entryId">The Id of an Entry</param>
        /// <param name="contributorId">The Id of a Contributor</param>
        /// <param name="attachmentId">The Id of an Attachment</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey AttachmentPreview(string bookId, string entryId, string contributorId, string attachmentId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);
            properties.Add(StoreKeyHelper.PROPERTY_ENTRY_ID, entryId);
            properties.Add(StoreKeyHelper.PROPERTY_CONTRIBUTOR_ID, contributorId);
            properties.Add(StoreKeyHelper.PROPERTY_ATTACHMENT_ID, attachmentId);

            return new StoreKey(StoreKeyTypes.AttachmentPreview, properties);
        }

        /// <summary>
        /// Returns the StoreKey for a Contributor
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <param name="contributorId">The Id of a Contributor</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey Contributor(string bookId, string contributorId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);
            properties.Add(StoreKeyHelper.PROPERTY_CONTRIBUTOR_ID, contributorId);

            return new StoreKey(StoreKeyTypes.Contributor, properties);
        }

        /// <summary>
        /// Returns the StoreKey for a list of Contributors
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey Contributors(string bookId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);

            return new StoreKey(StoreKeyTypes.Contributors, properties);
        }

        /// <summary>
        /// Returns the StoreKey for a BookShare
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <param name="bookShareId">The Id of a BookShare</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey BookShare(string bookId, string bookShareId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_SHARE_ID, bookShareId);

            return new StoreKey(StoreKeyTypes.BookShare, properties);
        }

        /// <summary>
        /// Returns the StoreKey for a list of BookShares
        /// </summary>
        /// <param name="bookId">The Id of a Book</param>
        /// <returns>The StoreKey</returns>
        public static StoreKey BookShares(string bookId)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            properties.Add(StoreKeyHelper.PROPERTY_BOOK_ID, bookId);

            return new StoreKey(StoreKeyTypes.BookShares, properties);
        }

        /// <summary>
        /// Gets the StoreKey as a string using the StoreKeyHelper
        /// </summary>
        /// <returns>The StoreKey as a string</returns>
        public override string ToString()
        {
            return StoreKeyHelper.Convert(this);
        }

        private StoreKey() { }
    }
}
