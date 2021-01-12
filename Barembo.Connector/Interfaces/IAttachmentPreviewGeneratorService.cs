using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// An AttachmentPreviewGeneratorService creates AttachmentPreviews for an Attachment of an Entry.
    /// For Images, it generates one thumbnail. For Videos it creates several thumbnails at different
    /// timestamps to create a quick glimpse at the whole video.
    /// </summary>
    public interface IAttachmentPreviewGeneratorService
    {
        /// <summary>
        /// Generates an AttachmentPreview for an Attachment
        /// </summary>
        /// <param name="attachment">The attachment-metadata</param>
        /// <param name="attachmentBinary">The attachment-binary as stream</param>
        /// <returns>The generated AttachmentPreview</returns>
        Task<AttachmentPreview> GeneratePreviewAsync(Attachment attachment, Stream attachmentBinary, string filePath);
    }
}
