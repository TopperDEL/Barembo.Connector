using Barembo.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.Models
{
    /// <summary>
    /// An entry of a book.
    /// </summary>
    public class Entry
    {
        /// <summary>
        /// The ID of an entry.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The header of an entry.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// The body of an entry.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The version of that entry - in case there needs to happen some migration for future adjustments.
        /// </summary>
        public string Version { get; set; } = VersionHelper.CURRENT_VERSION;

        /// <summary>
        /// The creation date of the entry.
        /// </summary>
        public DateTime CreationDate { get; set; } = DateTime.Now;

        /// <summary>
        /// The base64-encoded string of the thumbnail for this entry.
        /// </summary>
        public string ThumbnailBase64 { get; set; }

        /// <summary>
        /// A list of attachments of this entry. The order in that list defines the order
        /// of the attachments.
        /// </summary>
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        internal Entry() { }
    }
}
