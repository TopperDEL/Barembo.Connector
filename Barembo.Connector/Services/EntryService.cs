using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Services
{
    public class EntryService : IEntryService
    {
        IEntryStoreService _entryStoreService;
        IAttachmentStoreService _attachmentStoreService;

        public EntryService(IEntryStoreService entryStoreService, IAttachmentStoreService attachmentStoreService)
        {
            _entryStoreService = entryStoreService;
            _attachmentStoreService = attachmentStoreService;
        }

        public async Task<bool> AddAttachmentAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, bool setAsThumbnail)
        {
            var success = await _attachmentStoreService.SaveFromStreamAsync(entryReference, attachment, attachmentBinary);
            if (success)
            {
                entry.Attachments.Add(attachment);
                var successEntry = await _entryStoreService.SaveAsync(entryReference, entry);
                if (successEntry)
                    return true;
                else
                {
                    entry.Attachments.Remove(attachment);
                    return false;
                }
            }
            else
                return false;
        }

        public async Task<EntryReference> AddEntryToBookAsync(BookReference bookReference, Entry entry)
        {
            EntryReference entryReference = new EntryReference();
            entryReference.BookReference = bookReference;
            entryReference.EntryId = entry.Id;
            entryReference.EntryKey = StoreKey.Entry(bookReference.BookId, entry.Id, bookReference.ContributorId).ToString();

            var success = await _entryStoreService.SaveAsync(entryReference, entry);

            if (success)
                return entryReference;
            else
                throw new EntryCouldNotBeSavedException();
        }

        public async Task<Entry> CreateEntryAsync(string header)
        {
            return await CreateEntryAsync(header, "");
        }

        public async Task<Entry> CreateEntryAsync(string header, string body)
        {
            Entry entry = new Entry();
            entry.Header = header;
            entry.Body = body;

            return entry;
        }

        public async Task<IEnumerable<EntryReference>> ListEntriesAsync(BookReference bookReference)
        {
            return await _entryStoreService.ListAsync(bookReference);
        }

        public async Task<Stream> LoadAttachmentAsync(EntryReference entryReference, Attachment attachment)
        {
            return await _attachmentStoreService.LoadAsStreamAsync(entryReference, attachment);
        }

        public async Task<Entry> LoadEntryAsync(EntryReference entryReference)
        {
            return await _entryStoreService.LoadAsync(entryReference);
        }

        public async Task<bool> SaveEntryAsync(EntryReference entryReference, Entry entry)
        {
            return await _entryStoreService.SaveAsync(entryReference, entry);
        }
    }
}
