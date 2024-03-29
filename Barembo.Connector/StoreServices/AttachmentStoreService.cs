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
    public class AttachmentStoreService : IAttachmentStoreService
    {
        readonly IStoreService _storeService;

        public AttachmentStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<bool> DeleteAsync(EntryReference entryRef, Attachment attachment)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Attachment(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachment.Id));

            if (!entryInfo.ObjectExists)
                throw new AttachmentNotExistsException();

            return await _storeService.DeleteObjectAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Attachment(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachment.Id));
        }

        public async Task<Stream> LoadAsStreamAsync(EntryReference entryRef, Attachment attachment)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Attachment(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachment.Id));

            if (!entryInfo.ObjectExists)
                throw new AttachmentNotExistsException();

            return await _storeService.GetObjectAsStreamAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Attachment(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachment.Id));
        }

        public async Task<bool> SaveFromStreamAsync(EntryReference entryRef, Attachment attachmentToSave, Stream attachmentBinary, string filePath)
        {
            return await _storeService.PutObjectFromStreamAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Attachment(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId, attachmentToSave.Id), attachmentBinary, filePath);
        }
    }
}
