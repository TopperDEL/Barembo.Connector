using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IBackgroundActionService
    {
        /// <summary>
        /// Returns true, if BackgroundActions are being processed
        /// </summary>
        bool Processing { get; }

        /// <summary>
        /// Starts the processing of BackgroundActions
        /// </summary>
        void ProcessActionsInBackground();

        /// <summary>
        /// Stops the processing of BackgroundActions
        /// </summary>
        void StopProcessingInBackground();

        /// <summary>
        /// Add an attachment to an entry in background
        /// </summary>
        /// <param name="entryReference">The reference to an entry</param>
        /// <param name="attachment">The attachment to add</param>
        /// <param name="filePath">The path to the file</param>
        /// <returns>true, if it could be marked for background processing; false if not</returns>
        Task<bool> AddAttachmentInBackgroundAsync(EntryReference entryReference, Attachment attachment, string filePath);

        /// <summary>
        /// Sets a thumbnail on an entry in background
        /// </summary>
        /// <param name="entryReference">The reference to an entry</param>
        /// <param name="attachment">The attachment to add</param>
        /// <param name="filePath">The path to the file</param>
        /// <returns>true, if it could be marked for background processing; false if not</returns>
        Task<bool> SetThumbnailInBackgroundAsync(EntryReference entryReference, Attachment attachment, string filePath);
    }
}
