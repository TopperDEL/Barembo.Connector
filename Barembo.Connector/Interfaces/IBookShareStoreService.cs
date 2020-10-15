using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.Interfaces
{
    /// <summary>
    /// A BookShareStoreService is responsible for saving, loading and listing of all
    /// BookShares.
    /// </summary>
    public interface IBookShareStoreService
    {
        /// <summary>
        /// Saves a BookShare to the given StoreKey with the given StoreAccess and returns
        /// a BookShareReference. The latte can be shared e.g. by a QR-code.
        /// </summary>
        /// <param name="storeAccess">The StoreAccess to use</param>
        /// <param name="bookShare">The BookShare</param>
        /// <returns>The BookShareReference</returns>
        Task<BookShareReference> SaveBookShareAsync(StoreAccess storeAccess, BookShare bookShare);

        /// <summary>
        /// Loads a BookShare from a given BookShareReference
        /// </summary>
        /// <param name="bookShareReference">The BookShareReference</param>
        /// <returns>The BookShare if it exists; raises BookShareNotFoundException if not</returns>
        Task<BookShare> LoadBookShareAsync(BookShareReference bookShareReference);

        /// <summary>
        /// Lists all current BookShares for a given StoreAccess and a given Book
        /// </summary>
        /// <param name="storeAccess">The StoreAccess to use</param>
        /// <param name="bookReference">The BookReference to the Book to use</param>
        /// <returns>A list of all existing BookShareReferences</returns>
        Task<IEnumerable<BookShareReference>> ListBookSharesAsync(StoreAccess storeAccess, BookReference bookReference);
    }
}
