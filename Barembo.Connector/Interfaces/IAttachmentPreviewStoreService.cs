﻿using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// An AttachmentPreviewStoreService is responsible for loading and saving Previews of Attachments of an Entry.
    /// </summary>
    public interface IAttachmentPreviewStoreService
    {
        /// <summary>
        /// Loads an Attachment.
        /// </summary>
        /// <param name="entryRef">The EntryReference of an Attachment-Preview to load</param>
        /// <param name="attachment">The attachment to load the preview for</param>
        /// <returns>The Attachment-Preview if it exists, otherwise throws a AttachmentPreviewNotExistsException</returns>
        Task<AttachmentPreview> LoadAsync(EntryReference entryRef, Attachment attachment);

        /// <summary>
        /// Saves an Attachment-Preview using an EntryReference and the binary data as stream.
        /// </summary>
        /// <param name="entryRef">The EntryReference of an Entry to save</param>
        /// <param name="attachmentToSave">The Attachment-Preview-Metadata to save</param>
        /// <param name="attachmentPreview">The attachment-preview to save</param>
        /// <param name="filePath">The path to the file on the device</param>
        /// <returns>true, if the Attachment-preview could be saved</returns>
        Task<bool> SaveAsync(EntryReference entryRef, Attachment attachmentToSave, AttachmentPreview attachmentPreview, string filePath);
    }
}
