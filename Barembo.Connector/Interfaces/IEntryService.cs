﻿using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IEntryService
    {
        Entry CreateEntry(string header);

        Entry CreateEntry(string header, string body);

        Task<EntryReference> AddEntryToBookAsync(BookReference bookReference, Entry entry);

        Task<bool> AddAttachmentAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath);
        Task<bool> SetThumbnailAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath);

        Task<bool> SaveEntryAsync(EntryReference entryReference, Entry entry);

        Task<IEnumerable<EntryReference>> ListEntriesAsync(BookReference bookReference);

        Task<Entry> LoadEntryAsync(EntryReference entryReference);

        Task<Entry> LoadEntryAsync(EntryReference entryReference, bool ignoreBuffer);

        void LoadEntryAsSoonAsPossible(EntryReference entryReference, ElementLoadedDelegate<Entry> elementLoaded, ElementLoadingDequeuedDelegate loadingDequeued);

        Task<Stream> LoadAttachmentAsync(EntryReference entryReference, Attachment attachment);
        Task<AttachmentPreview> LoadAttachmentPreviewAsync(EntryReference entryReference, Attachment attachment);

        Task<bool> AddAttachmentFromBackgroundActionAsync(BackgroundAction backgroundAction);
        Task<bool> SetThumbnailFromBackgroundActionAsync(BackgroundAction backgroundAction);

        Task<bool> DeleteEntryAsync(EntryReference entryReference);
    }
}
