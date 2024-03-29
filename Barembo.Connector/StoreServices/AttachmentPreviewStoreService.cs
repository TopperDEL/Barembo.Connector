﻿using Barembo.Exceptions;
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

        public async Task<bool> DeleteAsync(EntryReference entryRef, Attachment attachmentToDelete)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachmentToDelete.Id));

            if (!entryInfo.ObjectExists)
                throw new AttachmentPreviewNotExistsException();

            return await _storeService.DeleteObjectAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachmentToDelete.Id));
        }

        public async Task<AttachmentPreview> LoadAsync(EntryReference entryRef, Attachment attachment)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachment.Id));

            if (!entryInfo.ObjectExists)
                throw new AttachmentPreviewNotExistsException();

            return await _storeService.GetObjectFromJsonAsync<AttachmentPreview>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachment.Id));
        }

        public async Task<bool> SaveAsync(EntryReference entryRef, Attachment attachmentToSave, AttachmentPreview attachmentPreview)
        {
            return await _storeService.PutObjectAsJsonAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.AttachmentPreview(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachmentToSave.Id), attachmentPreview);
        }
    }
}
