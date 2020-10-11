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
        /// <param name="ownerName">The name of the owner</param>
        /// <returns>The created BookShelf</returns>
        Task<BookShelf> CreateAndSaveBookShelfAsync(string ownerName);

        /// <summary>
        /// Loads a BookShelf
        /// </summary>
        /// <param name="access">The access to load the BookShelf from</param>
        /// <returns>The BookShelf if it could be loaded, throws BookShelfNotFoundException if not</returns>
        Task<BookShelf> LoadBookShelfAsync(StoreAccess access);

        /// <summary>
        /// Adds an own Book to the BookShelf and saves the BookShelf
        /// </summary>
        /// <param name="book">The Book to add</param>
        /// <returns>true, if the Book could be added; false if not</returns>
        Task<bool> AddOwnBookToBookShelfAndSaveAsync(Book book);

        /// <summary>
        /// Adds a shared Book to the BookShelf and saves the BookShelf
        /// </summary>
        /// <param name="shareInfo">The shared information about the Book</param>
        /// <returns>true, if the Book could be added; false if not</returns>
        Task<bool> AddSharedBookToBookShelfAndSaveAsync(BookShareInfo shareInfo);
    }
}
