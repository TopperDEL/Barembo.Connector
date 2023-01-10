using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class BackgroundEntryService : EntryService
    {
        readonly IStoreBuffer _storeBuffer;

        public BackgroundEntryService(IEntryStoreService entryStoreService, IAttachmentStoreService attachmentStoreService, IAttachmentPreviewStoreService attachmentPreviewStoreService, IAttachmentPreviewGeneratorService attachmentPreviewGeneratorService, IThumbnailGeneratorService thumbnailGeneratorService, IFileAccessHelper fileAccessHelper, IStoreBuffer storeBuffer) :
            base(entryStoreService, attachmentStoreService, attachmentPreviewStoreService, attachmentPreviewGeneratorService, thumbnailGeneratorService, fileAccessHelper)
        {
            _storeBuffer = storeBuffer;
        }

        public override async Task<bool> AddAttachmentAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath)
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

        public override async Task<bool> SetThumbnailAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, string filePath)
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
    }
}
