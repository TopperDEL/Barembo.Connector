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
        /// <param name="storeKey">The StoreKey to use</param>
        /// <param name="storeAccess">The StoreAccess to use</param>
        /// <param name="bookShare">The BookShare</param>
        /// <returns>The BookShareReference</returns>
        Task<BookShareReference> SaveBookShareAsync(StoreKey storeKey, StoreAccess storeAccess, BookShare bookShare);

        /// <summary>
        /// Loads a BookShare from a given BookShareReference
        /// </summary>
        /// <param name="bookShareReference">The BookShareReference</param>
        /// <returns>The BookShare if it exists; raises BookShareNotFoundException if not</returns>
        Task<BookShare> LoadBookShareAsync(BookShareReference bookShareReference);

        /// <summary>
        /// Lists all current BookShares for a given StoreAccess
        /// </summary>
        /// <param name="storeAccess">The StoreAccess to use</param>
        /// <returns>A list of all existing BookShares</returns>
        Task<IEnumerable<BookShare>> ListBookSharesAsync(StoreAccess storeAccess);
    }
}
