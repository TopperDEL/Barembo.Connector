using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Barembo.Services
{
    public class EntryService : IEntryService
    {
        private IStoreService _storeService;

        public EntryService(IStoreService storeService)
        {
            _storeService = storeService;
        }

        public async Task<IEnumerable<EntryReference>> ListAsync(BookReference bookRef)
        {
            var entries = await _storeService.ListObjectsAsync(new StoreAccess(bookRef.AccessGrant), StoreKey.Entries(bookRef.BookId));

            return entries.Select(e => new EntryReference() { BookReference = bookRef, EntryKey = e.Key, EntryId = e.Id });
        }

        public async Task<Entry> LoadAsync(EntryReference entryRef)
        {
            var entryInfo = await _storeService.GetObjectInfoAsync(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId));

            if (!entryInfo.ObjectExists)
                throw new EntryNotExistsException();

            return await _storeService.GetObjectFromJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId));
        }

        public async Task<bool> SaveAsync(EntryReference entryRef, Entry entryToSave)
        {
            return await _storeService.PutObjectAsJsonAsync<Entry>(new StoreAccess(entryRef.BookReference.AccessGrant), StoreKey.Entry(entryRef.BookReference.BookId, entryRef.EntryId, entryRef.BookReference.ContributorId), entryToSave);
        }
    }
}
