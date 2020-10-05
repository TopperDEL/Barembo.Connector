using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A BookShelfService is responsible for loading and saving a BookShelf.
    /// </summary>
    public interface IBookShelfService
    {
        /// <summary>
        /// Loads the BookShelf of the user. If none exists, a NoBookShelfExistsException gets thrown.
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <returns>The BookShelf or a NoBookShelfExistsException</returns>
        Task<BookShelf> LoadAsync(StoreAccess access);

        /// <summary>
        /// Saves the BookShelf of the user.
        /// </summary>
        /// <param name="access">The access to use</param>
        /// <param name="bookShelf">The BookShelf to save</param>
        /// <returns></returns>
        Task<bool> SaveAsync(StoreAccess access, BookShelf bookShelf);
    }
}
