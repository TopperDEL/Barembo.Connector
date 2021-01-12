using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.StoreServices
{
    public class AttachmentPreviewStoreService : IAttachmentPreviewStoreService
    {
        readonly IStoreService _storeService;

        public AttachmentPreviewStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<Stream> LoadAsStreamAsync(EntryReference entryRef, Attachment attachment)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, attachment.Id));

            if (!entryInfo.ObjectExists)
                throw new AttachmentPreviewNotExistsException();

            return await _storeService.GetObjectAsStreamAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, attachment.Id));
        }

        public async Task<bool> SaveFromStreamAsync(EntryReference entryRef, Attachment attachmentToSave, Stream attachmentBinary)
        {
            return await _storeService.PutObjectFromStreamAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, attachmentToSave.Id), attachmentBinary);
        }
    }
}
