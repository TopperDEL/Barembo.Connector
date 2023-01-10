using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class BackgroundEntryService : IEntryService
    {
        readonly IStoreBuffer _storeBuffer;
        readonly IEntryService _entryService;

        public BackgroundEntryService(IEntryStoreService entryStoreService, IAttachmentStoreService attachmentStoreService, IAttachmentPreviewStoreService attachmentPreviewStoreService, IAttachmentPreviewGeneratorService attachmentPreviewGeneratorService, IThumbnailGeneratorService thumbnailGeneratorService, IFileAccessHelper fileAccessHelper, IStoreBuffer storeBuffer)
        {
            _storeBuffer = storeBuffer;
            _entryService = new EntryService(entryStoreService, attachmentStoreService, attachmentPreviewStoreService, attachmentPreviewGeneratorService, thumbnailGeneratorService, fileAccessHelper);
        }

        public async Task<bool> AddAttachmentAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "EntryReference", entryReference },
                    { "Attachment", attachment },
                    { "FilePath", filePath }
                };

                BackgroundAction addAttachmentAction = new BackgroundAction(BackgroundActionTypes.AddAttachment, parameters);

                await _storeBuffer.AddBackgroundAction(addAttachmentAction);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddAttachmentFromBackgroundActionAsync(BackgroundAction backgroundAction)
        {
            return await _entryService.AddAttachmentFromBackgroundActionAsync(backgroundAction);
        }

        public async Task<EntryReference> AddEntryToBookAsync(BookReference bookReference, Entry entry)
        {
            return await AddEntryToBookAsync(bookReference, entry);
        }

        public Entry CreateEntry(string header)
        {
            return _entryService.CreateEntry(header);
        }

        public Entry CreateEntry(string header, string body)
        {
            return _entryService.CreateEntry(header, body);
        }

        public async Task<IEnumerable<EntryReference>> ListEntriesAsync(BookReference bookReference)
        {
            return await _entryService.ListEntriesAsync(bookReference);
        }

        public async Task<Stream> LoadAttachmentAsync(EntryReference entryReference, Attachment attachment)
        {
            return await LoadAttachmentAsync(entryReference, attachment);
        }

        public async Task<AttachmentPreview> LoadAttachmentPreviewAsync(EntryReference entryReference, Attachment attachment)
        {
            return await LoadAttachmentPreviewAsync(entryReference, attachment);
        }

        public void LoadEntryAsSoonAsPossible(EntryReference entryReference, ElementLoadedDelegate<Entry> elementLoaded, ElementLoadingDequeuedDelegate loadingDequeued)
        {
            _entryService.LoadEntryAsSoonAsPossible(entryReference, elementLoaded, loadingDequeued);
        }

        public async Task<Entry> LoadEntryAsync(EntryReference entryReference)
        {
            return await LoadEntryAsync(entryReference);
        }

        public async Task<bool> SaveEntryAsync(EntryReference entryReference, Entry entry)
        {
            return await _entryService.SaveEntryAsync(entryReference, entry);
        }

        public async Task<bool> SetThumbnailAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath)
        {
            try
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "EntryReference", entryReference },
                    { "Attachment", attachment },
                    { "FilePath", filePath }
                };

                BackgroundAction addAttachmentAction = new BackgroundAction(BackgroundActionTypes.SetThumbnail, parameters);

                await _storeBuffer.AddBackgroundAction(addAttachmentAction);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetThumbnailFromBackgroundActionAsync(BackgroundAction backgroundAction)
        {
            return await _entryService.SetThumbnailFromBackgroundActionAsync(backgroundAction);
        }
    }
}
