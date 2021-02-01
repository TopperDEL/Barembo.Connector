using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// An AttachmentStoreService is responsible for loading and saving Attachments of an Entry.
    /// </summary>
    public interface IAttachmentStoreService
    {
        /// <summary>
        /// Loads an Attachment.
        /// </summary>
        /// <param name="entryRef">The EntryReference of an Attachment to load</param>
        /// <param name="attachment">The attachment to load</param>
        /// <returns>The Attachment as a Stream if it exists, otherwise throws a AttachmentNotExistsException</returns>
        Task<Stream> LoadAsStreamAsync(EntryReference entryRef, Attachment attachment);

        /// <summary>
        /// Saves an Attachment using an EntryReference and the binary data as stream.
        /// </summary>
        /// <param name="entryRef">The EntryReference of an Entry to save</param>
        /// <param name="attachmentToSave">The Attachment-Metadata to save</param>
        /// <param name="attachmentBinary">The binary data of the attachment as stream</param>
        /// <param name="filePath">The path to the file on the device</param>
        /// <returns>true, if the Attachment could be saved</returns>
        Task<bool> SaveFromStreamAsync(EntryReference entryRef, Attachment attachmentToSave, Stream attachmentBinary, string filePath);
    }
}
