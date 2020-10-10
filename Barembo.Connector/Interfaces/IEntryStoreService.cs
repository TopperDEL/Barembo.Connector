using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// An EntryService is responsible for loading and saving an Entry
    /// </summary>
    public interface IEntryStoreService
    {
        /// <summary>
        /// Loads an Entry.
        /// </summary>
        /// <param name="entryRef">The EntryReference of an Entry to load</param>
        /// <returns>The Entry if it exists, otherwise throws a EntryNotExistsException</returns>
        Task<Entry> LoadAsync(EntryReference entryRef);

        /// <summary>
        /// Saves an Entry using an EntryReference.
        /// </summary>
        /// <param name="entryRef">The EntryReference of an Entry to save</param>
        /// <param name="entryToSave">The Entry to save</param>
        /// <returns>true, if the Entry could be saved</returns>
        Task<bool> SaveAsync(EntryReference entryRef, Entry entryToSave);

        /// <summary>
        /// Lists all Entries of a BookReference
        /// </summary>
        /// <param name="bookRef">The BookReference where the entries should be listed from</param>
        /// <returns>The list of entries</returns>
        Task<IEnumerable<EntryReference>> ListAsync(BookReference bookRef);
    }
}
