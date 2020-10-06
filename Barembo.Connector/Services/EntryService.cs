using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class EntryService : IEntryService
    {
        private IStoreService _storeService;

        public EntryService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<Entry> LoadAsync(EntryReference entryRef)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId));

            if (!entryInfo.ObjectExists)
                throw new EntryNotExistsException();

            return await _storeService.GetObjectFromJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId));
        }

        public async Task<bool> SaveAsync(EntryReference entryRef, Entry entryToSave)
        {
            return await _storeService.PutObjectAsJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId), entryToSave);
        }
    }
}
