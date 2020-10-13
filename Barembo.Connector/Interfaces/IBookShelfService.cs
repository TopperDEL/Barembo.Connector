using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A BookShelfService is responsible for managing a BookShelf
    /// </summary>
    public interface IBookShelfService
    {
        /// <summary>
        /// Creates and saves a new BookShelf for a given Owner-Name
        /// </summary>
        /// <param name="storeAccess">The StoreAccess to use</param>
        /// <param name="ownerName">The name of the owner</param>
        /// <returns>The created BookShelf or a BookShelfCouldNotBeSavedException</returns>
        Task<BookShelf> CreateAndSaveBookShelfAsync(StoreAccess storeAccess, string ownerName);

        /// <summary>
        /// Loads a BookShelf
        /// </summary>
        /// <param name="access">The access to load the BookShelf from</param>
        /// <returns>The BookShelf if it could be loaded, throws BookShelfNotFoundException if not</returns>
        Task<BookShelf> LoadBookShelfAsync(StoreAccess access);

        /// <summary>
        /// Adds an own Book to the BookShelf and saves the BookShelf
        /// </summary>
        /// <param name="access">The StoreAccess for the BookShelf to use</param>
        /// <param name="book">The Book to add</param>
        /// <param name="contributor">The Contributor to this Book</param>
        /// <returns>true, if the Book could be added; false if not</returns>
        Task<bool> AddOwnBookToBookShelfAndSaveAsync(StoreAccess access, Book book, Contributor contributor);

        /// <summary>
        /// Adds a shared Book to the BookShelf and saves the BookShelf
        /// </summary>
        /// <param name="storeAccess">The StoreAccess for the BookShelf to use</param>
        /// <param name="bookShareReference">The reference to the shared information about the Book</param>
        /// <returns>true, if the Book could be added; false if not</returns>
        Task<bool> AddSharedBookToBookShelfAndSaveAsync(StoreAccess access, BookShareReference bookShareReference);
    }
}
