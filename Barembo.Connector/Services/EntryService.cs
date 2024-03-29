﻿using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Services
{
    public class EntryService : IEntryService, IDisposable
    {
        readonly IEntryStoreService _entryStoreService;
        readonly IAttachmentStoreService _attachmentStoreService;
        readonly IAttachmentPreviewStoreService _attachmentPreviewStoreService;
        readonly IAttachmentPreviewGeneratorService _attachmentPreviewGeneratorService;
        readonly IQueuedPriorityLoaderService<Entry> _queuedLoaderService;
        readonly IThumbnailGeneratorService _thumbnailGeneratorService;
        readonly IFileAccessHelper _fileAccessHelper;

        public EntryService(IEntryStoreService entryStoreService, IAttachmentStoreService attachmentStoreService, IAttachmentPreviewStoreService attachmentPreviewStoreService, IAttachmentPreviewGeneratorService attachmentPreviewGeneratorService, IThumbnailGeneratorService thumbnailGeneratorService, IFileAccessHelper fileAccessHelper)
        {
            _entryStoreService = entryStoreService;
            _attachmentStoreService = attachmentStoreService;
            _attachmentPreviewStoreService = attachmentPreviewStoreService;
            _attachmentPreviewGeneratorService = attachmentPreviewGeneratorService;
            _thumbnailGeneratorService = thumbnailGeneratorService;
            _fileAccessHelper = fileAccessHelper;

            _queuedLoaderService = new QueuedPriorityLoaderService<Entry>();
        }

        public async Task<bool> AddAttachmentAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath)
        {
            var successAttachment = await _attachmentStoreService.SaveFromStreamAsync(entryReference, attachment, attachmentBinary, filePath).ConfigureAwait(false);
            if (!successAttachment)
            {
                return false;
            }

            var preview = await _attachmentPreviewGeneratorService.GeneratePreviewAsync(attachment, attachmentBinary, filePath).ConfigureAwait(false);
            var successPreview = await _attachmentPreviewStoreService.SaveAsync(entryReference, attachment, preview).ConfigureAwait(false);
            if (!successPreview)
            {
                return false;
            }

            entry.Attachments.Add(attachment);

            var successEntry = await _entryStoreService.SaveAsync(entryReference, entry).ConfigureAwait(false);
            if (successEntry)
            {
                return true;
            }
            else
            {
                entry.Attachments.Remove(attachment);
                return false;
            }
        }

        public async Task<EntryReference> AddEntryToBookAsync(BookReference bookReference, Entry entry)
        {
            if (!bookReference.AccessRights.CanAddEntries)
            {
                throw new ActionNotAllowedException();
            }

            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            entryReference.EntryKey = StoreKey.Entry(bookReference.BookId, entry.Id, bookReference.ContributorId).ToString();

            var success = await _entryStoreService.SaveAsync(entryReference, entry).ConfigureAwait(false);

            if (success)
            {
                return entryReference;
            }
            else
            {
                throw new EntryCouldNotBeSavedException();
            }
        }

        public Entry CreateEntry(string header)
        {
            return CreateEntry(header, "");
        }

        public Entry CreateEntry(string header, string body)
        {
            Entry entry = new Entry();
            entry.Header = header;
            entry.Body = body;

            return entry;
        }

        public async Task<IEnumerable<EntryReference>> ListEntriesAsync(BookReference bookReference)
        {
            var list = await _entryStoreService.ListAsync(bookReference).ConfigureAwait(false);

            if (!bookReference.IsOwnBook() && !bookReference.AccessRights.CanReadEntries)
            {
                throw new ActionNotAllowedException();
            }

            if (!bookReference.IsOwnBook() && !bookReference.AccessRights.CanReadForeignEntries)
            {
                list = list.Where(e => e.EntryId.Contains(bookReference.ContributorId));
            }

            return list;
        }

        public async Task<Stream> LoadAttachmentAsync(EntryReference entryReference, Attachment attachment)
        {
            return await _attachmentStoreService.LoadAsStreamAsync(entryReference, attachment).ConfigureAwait(false);
        }

        public async Task<AttachmentPreview> LoadAttachmentPreviewAsync(EntryReference entryReference, Attachment attachment)
        {
            return await _attachmentPreviewStoreService.LoadAsync(entryReference, attachment).ConfigureAwait(false);
        }

        public async Task<Entry> LoadEntryAsync(EntryReference entryReference)
        {
            if (!entryReference.BookReference.IsOwnBook() && !entryReference.BookReference.AccessRights.CanReadEntries)
            {
                throw new ActionNotAllowedException();
            }
            if (!entryReference.BookReference.IsOwnBook() && !entryReference.BookReference.AccessRights.CanReadForeignEntries)
            {
                throw new ActionNotAllowedException();
            }

            return await _entryStoreService.LoadAsync(entryReference).ConfigureAwait(false);
        }

        public async Task<Entry> LoadEntryAsync(EntryReference entryReference, bool ignoreBuffer)
        {
            if (!entryReference.BookReference.IsOwnBook() && !entryReference.BookReference.AccessRights.CanReadEntries)
            {
                throw new ActionNotAllowedException();
            }
            if (!entryReference.BookReference.IsOwnBook() && !entryReference.BookReference.AccessRights.CanReadForeignEntries)
            {
                throw new ActionNotAllowedException();
            }

            return await _entryStoreService.LoadAsync(entryReference, ignoreBuffer).ConfigureAwait(false);
        }

        public void LoadEntryAsSoonAsPossible(EntryReference entryReference, ElementLoadedDelegate<Entry> elementLoaded, ElementLoadingDequeuedDelegate loadingDequeued)
        {
            _queuedLoaderService.LoadWithHighPriority(async () => await _entryStoreService.LoadAsync(entryReference).ConfigureAwait(false), elementLoaded, loadingDequeued);
        }

        public async Task<bool> SaveEntryAsync(EntryReference entryReference, Entry entry)
        {
            if (entryReference.BookReference.IsOwnBook() && !entryReference.BookReference.AccessRights.CanEditOwnEntries)
            {
                throw new ActionNotAllowedException();
            }
            if (!entryReference.BookReference.IsOwnBook() && !entryReference.BookReference.AccessRights.CanEditForeignEntries)
            {
                throw new ActionNotAllowedException();
            }

            return await _entryStoreService.SaveAsync(entryReference, entry).ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _queuedLoaderService.StopAllLoading(true);
        }

        public async Task<bool> SetThumbnailAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath)
        {
            string bytesBase64 = string.Empty;
            Stream streamToUse;
            try
            {
                attachmentBinary.Position = 0;
                streamToUse = attachmentBinary;
            }
            catch
            {
                streamToUse = await _fileAccessHelper.OpenFileAsync(filePath);
            }
            if (attachment.Type == AttachmentType.Image)
            {
                bytesBase64 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromImageAsync(streamToUse).ConfigureAwait(false);
            }
            else if (attachment.Type == AttachmentType.Video)
            {
                bytesBase64 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(streamToUse, 0f, filePath).ConfigureAwait(false);
            }
            else
            {
                return false;
            }

            if (!string.IsNullOrEmpty(bytesBase64))
            {
                entry.ThumbnailBase64 = bytesBase64;

                var successEntry = await _entryStoreService.SaveAsync(entryReference, entry).ConfigureAwait(false);
                if (successEntry)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }

        public async Task<bool> AddAttachmentFromBackgroundActionAsync(BackgroundAction backgroundAction)
        {
            var parameters = backgroundAction.GetParameters();
            EntryReference entryReference = (EntryReference)parameters["EntryReference"];
            Attachment attachment = (Attachment)parameters["Attachment"];
            string filePath = (string)parameters["FilePath"];

            //Load the entry
            var entry = await LoadEntryAsync(entryReference, true);

            //Get the attachment-stream
            var stream = await _fileAccessHelper.OpenFileAsync(filePath);

            return await AddAttachmentAsync(entryReference, entry, attachment, stream, filePath);
        }

        public async Task<bool> SetThumbnailFromBackgroundActionAsync(BackgroundAction backgroundAction)
        {
            var parameters = backgroundAction.GetParameters();
            EntryReference entryReference = (EntryReference)parameters["EntryReference"];
            Attachment attachment = (Attachment)parameters["Attachment"];
            string filePath = (string)parameters["FilePath"];

            //Load the entry
            var entry = await LoadEntryAsync(entryReference, true);

            //Get the attachment-stream
            var stream = await _fileAccessHelper.OpenFileAsync(filePath);

            return await SetThumbnailAsync(entryReference, entry, attachment, stream, filePath);
        }

        public async Task<bool> DeleteEntryAsync(EntryReference entryReference)
        {
            var entry = await LoadEntryAsync(entryReference);

            foreach (var attachment in entry.Attachments)
            {
                var attachmentDeleted = await _attachmentStoreService.DeleteAsync(entryReference, attachment);
                if (!attachmentDeleted)
                    return false;

                var attachmentPreviewDeleted = await _attachmentPreviewStoreService.DeleteAsync(entryReference, attachment);
                if (!attachmentPreviewDeleted)
                    return false;
            }
            return await _entryStoreService.DeleteAsync(entryReference);
        }
    }
}
