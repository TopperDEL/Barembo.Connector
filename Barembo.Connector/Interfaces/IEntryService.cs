using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    public interface IEntryService
    {
        Entry CreateEntry(string header);

        Entry CreateEntry(string header, string body);

        Task<EntryReference> AddEntryToBookAsync(BookReference bookReference, Entry entry);

        Task<bool> AddAttachmentAsync(EntryReference entryReference, Entry entry, Attachment attachment, Stream attachmentBinary, bool setAsThumbnail);

        Task<bool> SaveEntryAsync(EntryReference entryReference, Entry entry);

        Task<IEnumerable<EntryReference>> ListEntriesAsync(BookReference bookReference);

        Task<Entry> LoadEntryAsync(EntryReference entryReference);

        Task<Stream> LoadAttachmentAsync(EntryReference entryReference, Attachment attachment);
    }
}
