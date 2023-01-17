using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.StoreServices
{
    public class EntryStoreService : IEntryStoreService
    {
        readonly IStoreService _storeService;

        public EntryStoreService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<bool> DeleteAsync(EntryReference entryRef)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.EntryReference(entryRef.EntryKey));

            if (!entryInfo.ObjectExists)
                throw new EntryNotExistsException();

            return await _storeService.DeleteObjectAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.EntryReference(entryRef.EntryKey));
        }

        public async Task<IEnumerable<EntryReference>> ListAsync(BookReference bookRef)
        {
            var entries = await _storeService.ListObjectsAsync(new StoreAccess(bookRef.AccessGrant), StoreKey.Entries(bookRef.BookId), true);

            return entries.Select(e =>
                {
                    return new EntryReference
                    {
                        BookReference = bookRef,
                        EntryKey = e.Key,
                        EntryId = e.Id,
                        CreationDate = string.IsNullOrEmpty(e.MetaData.Value) ? 
                                        DateTime.MinValue : DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(e.MetaData.Value)).DateTime
                    };
                }
                ).OrderByDescending(e=>e.CreationDate);
        }

        public async Task<Entry> LoadAsync(EntryReference entryRef)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.EntryReference(entryRef.EntryKey));

            if (!entryInfo.ObjectExists)
                throw new EntryNotExistsException();

            return await _storeService.GetObjectFromJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.EntryReference(entryRef.EntryKey));
        }

        public async Task<Entry> LoadAsync(EntryReference entryRef, bool ignoreBuffer)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.EntryReference(entryRef.EntryKey));

            if (!entryInfo.ObjectExists)
                throw new EntryNotExistsException();

            return await _storeService.GetObjectFromJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.EntryReference(entryRef.EntryKey), ignoreBuffer);
        }

        public async Task<bool> SaveAsync(EntryReference entryRef, Entry entryToSave)
        {
            DateTimeOffset dToffset = new DateTimeOffset(entryToSave.CreationDate);
            StoreMetaData metaData = new StoreMetaData(StoreMetaData.STOREMETADATA_TIMESTAMP, dToffset.ToUnixTimeSeconds().ToString());
            return await _storeService.PutObjectAsJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), 
                                                                   StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId), 
                                                                   entryToSave,
                                                                   metaData);
        }
    }
}
